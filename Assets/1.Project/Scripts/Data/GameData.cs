using UnityEngine;

public class GameData
{
    public RWGlobalFactorConfig metaAttributeConfig = null;
    public RWTableDataSkill skillConfig = null;

    //public GameConfig config = null;
    //public TanukiCharacterConfig characterConfig = null;
    //public WeaponConfig weaponConfig = null;

    //public UserData userData = new UserData();
    //public DisplayData displayData = new DisplayData();
    //public InGameData inGame = new InGameData();
    //public OptionData option = new OptionData();
    //public TutorialData tutorialData = new TutorialData();

    //public ReactiveProperty<ELocalization> localization = new ReactiveProperty<ELocalization>(ELocalization.Korean);

    //public ReactiveProperty<bool> isActiveMouseMode = new ReactiveProperty<bool>(false);

    //public ELocalization Localization
    //{
    //    get
    //    {
    //        var result = ELocalization.None;
    //        if (PlayerPrefs.HasKey(config.localizationKey))
    //        {
    //            result = (ELocalization)PlayerPrefs.GetInt(config.localizationKey);
    //        }
    //        else
    //        {
    //            result = config.defaultLanguage;
    //        }

    //        return result;
    //    }
    //    set
    //    {
    //        if (localization.Value != abilityValue)
    //        {
    //            PlayerPrefs.SetInt(config.localizationKey, (int)abilityValue);
    //            PlayerPrefs.Save();
    //            localization.Value = abilityValue;
    //        }
    //    }
    //}

    public void Init()
    {
        metaAttributeConfig = Resources.Load<RWGlobalFactorConfig>("ScriptableObject/RWGlobalFactorConfig");
        skillConfig = Resources.Load<RWTableDataSkill>("ScriptableObject/TableDataSkill");
        //config = Resources.Load<GameConfig>("GameConfig/GameConfig");
        //characterConfig = Resources.Load<TanukiCharacterConfig>("GameConfig/CharacterConfig");
        //weaponConfig = Resources.Load<WeaponConfig>("GameConfig/WeaponConfig");
        //localization.Value = Localization;
        //RewardData.Init();
    }
}
