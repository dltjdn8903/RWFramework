using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IAddFunctor
{
    void Add(CompositeDisposable disposables);
}
public class AddFunctor<T> : IAddFunctor
{
    public Action<T> callbackAction;
    public IObservable<T> observable;

    private AddFunctor() { }

    public AddFunctor(Action<T> onAction)
    {
        callbackAction = onAction;
    }

    public AddFunctor(ReactiveProperty<T> TProperty, Action<T> onAction)
    {
        observable = TProperty;
        callbackAction = onAction;
    }

    public AddFunctor(IObservable<T> observable, Action<T> onAction)
    {
        this.observable = observable;
        callbackAction = onAction;
    }

    void IAddFunctor.Add(UniRx.CompositeDisposable disposables)
    {
        var observableObj = observable == null ? MessageBroker.Default.Receive<T>() : observable;
        var disposableObj = UniRx.ObservableExtensions.Subscribe(observableObj, callbackAction);
        disposableObj.AddTo(disposables);
    }
}


public class DisposableObjectFactory : MonoBehaviour
{
    //  Awake/Start <-> OnDestroy 에 할당/해제를 해야 하는 오브젝트 관리.
    private CompositeDisposable disposables = new CompositeDisposable();

    //  OnEnable <-> OnDisable 에 할당/해제를 해야 하는 오브젝트 관리. 
    private CompositeDisposable disposablesOnToggle = new CompositeDisposable();
    private List<IAddFunctor> addFunctorList = new List<IAddFunctor>();

    private void Start()
    {
        if (disposablesOnToggle.Count != addFunctorList.Count)
        {
            _subscribeOnEnable();
        }
    }

    private void OnEnable()
    {
        _subscribeOnEnable();
    }

    void _subscribeOnEnable()
    {
        disposablesOnToggle.Clear();
        foreach (var functor in addFunctorList)
        {
            functor.Add(disposablesOnToggle);
        }
    }

    private void OnDisable()
    {
        disposablesOnToggle.Clear();
    }


    private void OnDestroy()
    {
        disposables.Clear();
        disposablesOnToggle.Clear();
    }

    public static DisposableObjectFactory GetOrAdd(GameObject ownerObject)
    {
        var factory = ownerObject.GetComponent<DisposableObjectFactory>();
        if (null == factory)
        {
            factory = ownerObject.AddComponent<DisposableObjectFactory>();
        }

        return factory;
    }

    public void SubscribeMessage<T>(Action<T> action)
    {
        var observableObj = MessageBroker.Default.Receive<T>();
        var disposableObj = UniRx.ObservableExtensions.Subscribe(observableObj, action);
        disposableObj.AddTo(disposables);
    }

    public void SubscribeMessageOnToggle<T>(Action<T> action)
    {
        var addFunctor = new AddFunctor<T>(action);
        addFunctorList.Add(addFunctor);
    }

    public void SubscribeEvent<T>(IObservable<T> observableObj, Action<T> action)
    {
        var disposableObj = UniRx.ObservableExtensions.Subscribe(observableObj, action);
        disposableObj.AddTo(disposables);
    }

    public void SubscribeEventOnToggle<T>(IObservable<T> observableObj, Action<T> action)
    {
        var addFunctor = new AddFunctor<T>(observableObj, action);
        addFunctorList.Add(addFunctor);
    }

    //  ReactiveCollection 에서 value가 변경된것을 감지하려면
    //  add, remove, replace를 모두 observe해야 한다.
    public void ObserveReactiveCollection<TVal>(
        IReactiveCollection<TVal> container,
        Action<TVal> onAction
    )
    {
        UniRx.ObservableExtensions.Subscribe(
            container.ObserveAdd(),
            value =>
            {
                onAction(value.Value);
            }
        ).AddTo(disposables);

        UniRx.ObservableExtensions.Subscribe(
            container.ObserveRemove(),
            value =>
            {
                onAction(value.Value);
            }
        ).AddTo(disposables);

        UniRx.ObservableExtensions.Subscribe(
            container.ObserveReplace(),
            value =>
            {
                onAction(value.NewValue);
            }
        ).AddTo(disposables);
    }

    //  ReactiveDictionary 에서 value가 변경된것을 감지하려면
    //  add, replace를 모두 observe해야 한다.
    public void ObserveReactiveDictionary<TKey, TVal>(
        IReactiveDictionary<TKey, TVal> container,
        Action<TVal> onAction
    )
    {
        UniRx.ObservableExtensions.Subscribe(
            container.ObserveAdd(),
            value =>
            {
                onAction(value.Value);
            }
        ).AddTo(disposables);

        UniRx.ObservableExtensions.Subscribe(
            container.ObserveRemove(),
            value =>
            {
                onAction(value.Value);
            }
        ).AddTo(disposables);

        UniRx.ObservableExtensions.Subscribe(
            container.ObserveReplace(),
            value =>
            {
                onAction(value.NewValue);
            }
        ).AddTo(disposables);
    }

    public void ObserveReactiveBool(ReactiveProperty<bool> boolProperty, Action<bool> onAction)
    {
        UniRx.ObservableExtensions.Subscribe(
            boolProperty,
            value =>
            {
                onAction?.Invoke(value);
            }
        ).AddTo(disposables);
    }

    public void ObserveReactive<T>(ReactiveProperty<T> TProperty, Action<T> onAction)
    {
        UniRx.ObservableExtensions.Subscribe(
            TProperty,
            value =>
            {
                onAction?.Invoke(value);
            }
        ).AddTo(disposables);
    }

    public void ObserveReactiveOnToggle<T>(ReactiveProperty<T> TProperty, Action<T> onAction)
    {
        var addFunctor = new AddFunctor<T>(TProperty, onAction);
        addFunctorList.Add(addFunctor);
    }
}
