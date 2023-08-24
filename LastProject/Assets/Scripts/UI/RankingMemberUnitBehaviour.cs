using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingMemberUnitBehaviour : MonoBehaviour
{
    public Image CharacterClassImage;

    public TMP_Text NicknameText;

    public void SetRankingInfo(RankingUnitInfo rui)
    {
        NicknameText.text = rui.nickname;
        CharacterClassImage.sprite = CharacterClassManager.Instance.GetCharacterClassInfo(rui.characterClassId).CharacterClassImage;
    }

}
