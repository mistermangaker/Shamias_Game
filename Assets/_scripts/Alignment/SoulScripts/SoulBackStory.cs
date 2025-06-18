using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class Keywords
{
    public static readonly string BlankNameKeyWord = "[Blank]";
    public static readonly string NameFullKeyWord = "[SoulFullName]";
    public static readonly string SoulFirstNameKeyWord = "[SoulFirstName]";
    public static readonly string SoulLastNameKeyWord = "[SoulLastName]";
    public static readonly string SoulNickNameKeyWord = "[SoulNickName]";
    public static readonly string PronounKeyWord = "[SoulPronoun]";
    public static readonly string PossessiveKeyWord = "[SoulPossessive]";
    public static readonly string PersonalKeyWord = "[SoulPersonal]";
}


[Serializable]
public class SoulBackStory
{
 

    [Header("Names")]
    [field:SerializeField] public string SoulFullName {  get; private set; }

    [field: SerializeField] public string SoulFirstName { get; private set; }
    [field: SerializeField] public string SoulLastName { get; private set; }
    [field: SerializeField] public string SoulNickName { get; private set; }

    [Header("BackStory IDS")]
    [field: SerializeField] public string ChildHoodID { get; private set; }
    [field: SerializeField] public string AdultHoodID { get; private set; }
    [field: SerializeField] public string DeathCauseID { get; private set; }
    [field: SerializeField] public SoulGender Gender { get; private set; }

    [NonSerialized] private SoulBackStoryPart childHoodPart;
    public SoulBackStoryPart ChildHoodPart => childHoodPart;
    [NonSerialized] private SoulBackStoryPart adultHoodPart;
    public SoulBackStoryPart AdultHoodPart => adultHoodPart;
    [NonSerialized] private SoulBackStoryPart deathCausePart;
    public SoulBackStoryPart DeathCausePart => deathCausePart;

    public List<AlignmentAdjustment> ConnectedAlignments = new List<AlignmentAdjustment>();
    public Alignment GetDefaultAlignmentFromBackStory()
    {
        Alignment alignment = new Alignment();
        foreach(AlignmentAdjustment adjustment in ConnectedAlignments)
        {
            alignment.EffectRangeTowards(adjustment.amountToModify, adjustment.TypeToModify);
        }
        return alignment;
    }

    public static SoulBackStory Default
    {
        get
        {
            return new SoulBackStory("charles entertainment cheese", 
                DataBase.GetDefaultBackStoryIdFromLifeTimeTag(SoulBackStoryLifeTimeTag.ChildHood),
                 DataBase.GetDefaultBackStoryIdFromLifeTimeTag(SoulBackStoryLifeTimeTag.AdultHood),
                  DataBase.GetDefaultBackStoryIdFromLifeTimeTag(SoulBackStoryLifeTimeTag.DeathCause),
                  SoulGender.Male
                );

        }
    }

    public SoulBackStory(string fullName, SoulBackStoryPart childhood, SoulBackStoryPart adultHood, SoulBackStoryPart deathCause, SoulGender gender)
    {
        if (fullName == "")
        {
            Debug.LogError("empty name Recieved names can not be recieved");
            fullName = "charles entertainment cheese";
        }

        List<string> temp = fullName.Split().ToList();
        //Debug.Log(temp.Count);
        if (temp.Count >= 3)
        {
            this.SoulFullName = $"{temp[0]} {temp[1]} {temp[2]}";
            this.SoulFirstName = temp[0];
            this.SoulNickName = temp[1];
            this.SoulLastName = temp[2];

        }
        else if (temp.Count == 2)
        {
            this.SoulFullName = $"{temp[0]} \' \' {temp[1]}";
            this.SoulFirstName = temp[0];
            this.SoulNickName = "\' \'";
            this.SoulLastName = temp[1];
        }
        //Debug.Log(this.SoulFullName);

        childHoodPart = childhood;
        adultHoodPart = adultHood;
        deathCausePart = deathCause;

        ChildHoodID = childHoodPart.Id;
        AdultHoodID = adultHoodPart.Id;
        DeathCauseID = deathCausePart.Id;

        ConnectedAlignments.AddRange(childHoodPart.ConnectedAlignments);
        ConnectedAlignments.AddRange(adultHoodPart.ConnectedAlignments);
        ConnectedAlignments.AddRange(deathCausePart.ConnectedAlignments);
        Gender = gender;
    }

    public SoulBackStory(string fullName, string childhoodId, string adultHoodId, string deathCauseId , SoulGender gender)
    {
        if (fullName == "")
        {
            Debug.LogError("empty name Recieved names can not be recieved");
            fullName = "charles entertainment cheese";
        }

        if (!DataBase.BackStoryIdExists(childhoodId)) {
            Debug.LogWarning($"Invalid childHood backstory Id Detected {childhoodId} is not found in the resources folder");
            childhoodId = DataBase.GetDefaultBackStoryIdFromLifeTimeTag(SoulBackStoryLifeTimeTag.ChildHood);
                }
        if (!DataBase.BackStoryIdExists(adultHoodId))
        {
            Debug.LogWarning($"Invalid AdultHood backstory Id Detected {adultHoodId} is not found in the resources folder");
            childhoodId = DataBase.GetDefaultBackStoryIdFromLifeTimeTag(SoulBackStoryLifeTimeTag.AdultHood);
        }
        if (!DataBase.BackStoryIdExists(deathCauseId))
        {
            Debug.LogWarning($"Invalid Deathcause backstory Id Detected {deathCauseId} is not found in the resources folder");
            childhoodId = DataBase.GetDefaultBackStoryIdFromLifeTimeTag(SoulBackStoryLifeTimeTag.DeathCause);
        }
        
        List<string> temp = fullName.Split().ToList();
        Debug.Log(temp.Count);
        if (temp.Count >= 3)
        {
            this.SoulFullName = $"{temp[0]} {temp[1]} {temp[2]}";
            this.SoulFirstName = temp[0];
            this.SoulNickName = temp[1];
            this.SoulLastName = temp[2];

        }
        else if (temp.Count == 2)
        {
            this.SoulFullName = $"{temp[0]} \' \' {temp[1]}";
            this.SoulFirstName = temp[0];
            this.SoulNickName = "\' \'";
            this.SoulLastName = temp[1];
        }

        childHoodPart = DataBase.GetSoulBackStoryPartFromId(childhoodId);
        adultHoodPart = DataBase.GetSoulBackStoryPartFromId(adultHoodId);
        deathCausePart = DataBase.GetSoulBackStoryPartFromId(deathCauseId);

        ChildHoodID = childHoodPart.Id;
        AdultHoodID = adultHoodPart.Id;
        DeathCauseID = deathCausePart.Id;

        ConnectedAlignments.AddRange(childHoodPart.ConnectedAlignments);
        ConnectedAlignments.AddRange(adultHoodPart.ConnectedAlignments);
        ConnectedAlignments.AddRange(deathCausePart.ConnectedAlignments);
        Gender = gender;

    }



    public string GetFullBackStory()
    {
        if(ChildHoodID == "") return "Empty BackStory";
        string childhood = DataBase.GetSoulBackStoryPartFromId(ChildHoodID).BackGroundString;
        string adulthood = DataBase.GetSoulBackStoryPartFromId(AdultHoodID).BackGroundString;
        string deathCause = DataBase.GetSoulBackStoryPartFromId(DeathCauseID).BackGroundString;

        string mergedBackStory = $"{childhood}\n{adulthood}\n{deathCause}\n";

        mergedBackStory = mergedBackStory.Replace(Keywords.NameFullKeyWord, SoulFullName);

        mergedBackStory = mergedBackStory.Replace(Keywords.SoulFirstNameKeyWord, SoulFirstName);
        mergedBackStory = mergedBackStory.Replace(Keywords.SoulLastNameKeyWord, SoulLastName);
        mergedBackStory = mergedBackStory.Replace(Keywords.SoulNickNameKeyWord, SoulNickName);

        mergedBackStory = mergedBackStory.Replace(Keywords.PronounKeyWord, StringFormatter.Pronoun(Gender));
        mergedBackStory = mergedBackStory.Replace(Keywords.PossessiveKeyWord, StringFormatter.Posessive(Gender));
        mergedBackStory = mergedBackStory.Replace(Keywords.PersonalKeyWord, StringFormatter.Personal(Gender));
       
        return mergedBackStory;
    }


    public override string ToString()
    {
        return $"{SoulFullName} {ChildHoodID} {AdultHoodID} {DeathCauseID}";
    }
}
