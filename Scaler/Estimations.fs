module Scaler.Estimations

open Scaler.Models
open Scaler.Gamemastery

type AbsoluteCompare<'a> = int -> 'a -> ('a -> int) -> int
type IntervalCompare<'a> = int -> 'a -> ('a -> int) -> ('a -> int) -> double
type RatioCompare<'a> = int -> 'a -> ('a -> int) -> double

type ComparerOption<'a> =
  | Absolute of AbsoluteCompare<'a>
  | Interval of IntervalCompare<'a>
  | Ratio of RatioCompare<'a>

type LevelValueOption =
  | Absolute of (int -> int)
  | Interval of (int -> int) * (int -> int)
  | Ratio of (int -> int)

type StrikeEstimateCategory =
  | Low
  | Moderate
  | High
  | Extreme

let absoluteAttributeCompare attribute originalLevel estimator =
  abs ((estimator originalLevel) - attribute)

let ratioCompare attribute originalLevel estimator =
  (double attribute) / double (estimator originalLevel)

let absoluteAttributeSort attribute originalLevel estimators =
  Seq.sortBy (fun estimator -> absoluteAttributeCompare attribute originalLevel estimator) estimators

let intervalRatio value originalLevel estimatorLow estimatorHigh =
  let (low, high) = (absoluteAttributeCompare value originalLevel estimatorLow, absoluteAttributeCompare value originalLevel estimatorHigh)
  (double low) / double (low + high)

let estimateAbsoluteAttributeFromClosestFunction attribute originalLevel newLevel closestFunction =
  attribute - closestFunction originalLevel + closestFunction newLevel

let estimateIntervalAttributeFromClosestFunction attribute originalLevel newLevel (fLow: int -> int) (fHigh: int -> int) =
  intervalRatio attribute originalLevel fLow fHigh
  |> (*) (double (fHigh newLevel - fLow newLevel))
  |> round
  |> int
  |> (+) (fLow newLevel)

let estimateRatioAttributeFromClosestFunction attribute originalLevel newLevel closestFunction =
  closestFunction newLevel
  |> double
  |> (*) (ratioCompare attribute originalLevel closestFunction)
  |> int

let estimateAbsoluteAttribute attribute originalLevel newLevel functions =
  absoluteAttributeSort attribute originalLevel functions 
  |> Seq.head
  |> estimateAbsoluteAttributeFromClosestFunction attribute originalLevel newLevel

let intervalCompare value originalLevel estimatorLow estimatorHigh =
  if value <= estimatorLow originalLevel || value >= estimatorHigh originalLevel then None
  else intervalRatio value originalLevel estimatorLow estimatorHigh |> Some

let mixedSortValue attribute originalLevel levelValueOption =
  match levelValueOption with
  | Interval (fLow, fHigh) -> intervalCompare attribute originalLevel fLow fHigh
  | Absolute f -> 
    absoluteAttributeCompare attribute originalLevel f
    |> double
    |> Some
  | Ratio f ->
    ratioCompare attribute originalLevel f
    |> Some

let headBy func sequence =
  Seq.filter (fun x -> func x |> Option.isSome) sequence
  |> Seq.sortBy (fun x -> (func x).Value)
  |> Seq.head

let closestMixedAttributeComparer attribute originalLevel comparers =
  //Seq.filter (fun comparer -> (mixedSortValue attribute originalLevel comparer) |> Option.isSome) comparers
  //|> Seq.sortBy (fun comparer -> (mixedSortValue attribute originalLevel comparer).Value)
  //|> Seq.head
  headBy (mixedSortValue attribute originalLevel) comparers

let estimateLevelValueOption attribute originalLevel newLevel levelValueOption =
  match levelValueOption with
  | Interval (low, high) -> estimateIntervalAttributeFromClosestFunction attribute originalLevel newLevel low high
  | Absolute estimator -> estimateAbsoluteAttributeFromClosestFunction attribute originalLevel newLevel estimator
  | Ratio estimator -> estimateRatioAttributeFromClosestFunction attribute originalLevel newLevel estimator

let estimateLevelValueOptionAttribute attribute originalLevel newLevel levelValueOptions =
  levelValueOptions
  |> closestMixedAttributeComparer attribute originalLevel
  |> estimateLevelValueOption attribute originalLevel newLevel

let estimatePerception (perception: int) (originalLevel: int) (newLevel: int) =
  seq { terriblePerception; lowPerception; moderatePerception; highPerception; extremePerception }
  |> estimateAbsoluteAttribute perception originalLevel newLevel

let estimateAbilityModifier modifier originalLevel newLevel =
  if originalLevel < 1 then seq { 
    terribleAbilityModifier;
    lowAbilityModifier;
    moderateAbilityModifier;
    highAbilityModifier;
  } else seq {
    terribleAbilityModifier;
    lowAbilityModifier;
    moderateAbilityModifier;
    highAbilityModifier;
    extremeAbilityModifier
  }
  |> estimateAbsoluteAttribute modifier originalLevel newLevel

let estimateArmorClass modifier originalLevel newLevel =
  seq { lowArmorClass; moderateArmorClass; highArmorClass; extremeArmorClass }
  |> estimateAbsoluteAttribute modifier originalLevel newLevel

let estimateSavingThrow modifier originalLevel newLevel =
  seq { terribleSavingThrow; lowSavingThrow; moderateSavingThrow; highSavingThrow; extremeSavingThrow }
  |> estimateAbsoluteAttribute modifier originalLevel newLevel

let estimateStrikeHit modifier originalLevel newLevel =
  seq { lowStrikeHit; moderateStrikeHit; highStrikeHit; extremeStrikeHit; }
  |> estimateAbsoluteAttribute modifier originalLevel newLevel

let estimateSpellHit modifier originalLevel newLevel =
  seq { moderateSpellHit; highSpellHit; extremeSpellHit }
  |> estimateAbsoluteAttribute modifier originalLevel newLevel

let estimateSpellDifficultyClass modifier originalLevel newLevel =
  seq { moderateSpellDifficultyClass; highSpellDifficultyClass; extremeSpellDifficultyClass }
  |> estimateAbsoluteAttribute modifier originalLevel newLevel

let private defaultDiceSize = 6
let possibleDiceSizes = Seq.init 5 (fun n -> 2 * (n+2))
let diceSizeToAverage size = double (size+1) / 2.0
let possibleDiceAverages = Seq.map diceSizeToAverage possibleDiceSizes
let toDecimal seq = Seq.map (fun x -> decimal x) seq

let averageToMatchingDamageDiceSize average (size: int) =
  { 
    bonus = average % (diceSizeToAverage size) |> int |> Some;
    count = int (average / (diceSizeToAverage size));
    size = size
  }

let private _averageToPossibleDamageDice average sizes =
  Seq.map (fun size -> averageToMatchingDamageDiceSize average size) sizes
  |> Seq.where (fun possible -> possible.count > 0)

let averageToPossibleDamageDice average sizes =
  match average with
  | average when average < 3.0 -> seq { averageToMatchingDamageDiceSize average defaultDiceSize }
  | _ -> _averageToPossibleDamageDice average sizes

let averageToNearestDamageDice average sizes =
  averageToPossibleDamageDice average sizes
  |> Seq.sortBy (fun nearby -> nearby.bonus)
  |> Seq.head

let estimateSkill modifier originalLevel newLevel =
  seq {
    LevelValueOption.Absolute terribleSkill;
    LevelValueOption.Interval (terribleSkill, lowSkill);
    LevelValueOption.Absolute lowSkill;
    LevelValueOption.Absolute moderateSkill;
    LevelValueOption.Absolute highSkill;
    LevelValueOption.Absolute extremeSkill
  }
  |> estimateLevelValueOptionAttribute modifier originalLevel newLevel

let estimateResistanceOrWeakness modifier originalLevel newLevel =
  seq {
    LevelValueOption.Absolute moderateResistance;
    LevelValueOption.Interval (moderateResistance, highResistance);
    LevelValueOption.Absolute highResistance;
  }
  |> estimateLevelValueOptionAttribute modifier originalLevel newLevel
  |> max 1

let estimateHitPoint hitPoint originalLevel newLevel =
  double (moderateHitPoint newLevel) * double hitPoint / double (moderateHitPoint originalLevel)
  |> round
  |> int

let estimateDieSizeFromCategory estimateCategory dieSize originalLevel newLevel =
  match estimateCategory with
  | StrikeEstimateCategory.Low -> estimateAbsoluteAttributeFromClosestFunction dieSize originalLevel newLevel lowStrikeDamageDiceSize
  | StrikeEstimateCategory.Moderate -> estimateAbsoluteAttributeFromClosestFunction dieSize originalLevel newLevel moderateStrikeDamageDiceSize
  | StrikeEstimateCategory.High -> estimateAbsoluteAttributeFromClosestFunction dieSize originalLevel newLevel highStrikeDamageDiceSize
  | StrikeEstimateCategory.Extreme -> estimateAbsoluteAttributeFromClosestFunction dieSize originalLevel newLevel extremeStrikeDamageDiceSize
  |> min 12
  |> max 4

let estimateAverageStrikeDamage averageDamage dieSize originalLevel newLevel =
  let (averageLvo, category) = 
    seq {
      LevelValueOption.Ratio (lowStrikeDamageAverage >> min 1), StrikeEstimateCategory.Low;
      LevelValueOption.Interval (lowStrikeDamageAverage, moderateStrikeDamageAverage), StrikeEstimateCategory.Moderate;
      LevelValueOption.Interval (moderateStrikeDamageAverage, highStrikeDamageAverage), StrikeEstimateCategory.High;
      LevelValueOption.Interval (highStrikeDamageAverage, extremeStrikeDamageAverage), StrikeEstimateCategory.Extreme;
      LevelValueOption.Ratio (extremeStrikeDamageAverage >> max 1), StrikeEstimateCategory.Extreme;
    }
    |> headBy (fun (lvo, _) -> mixedSortValue averageDamage originalLevel lvo)

  let scaledAverage = estimateLevelValueOption averageDamage originalLevel newLevel averageLvo
  let size = estimateDieSizeFromCategory category dieSize originalLevel newLevel
  let count = strikeDamageDiceCount newLevel
  let bonus = scaledAverage - averageDiceDamage count size

  { count = count; size = size; bonus = Some bonus}
  
// need to test the below more

let estimateUnlimitedUseAreaDamage averageDamage originalLevel newLevel =
  seq {
    LevelValueOption.Ratio (unlimitedUseAreaDamageAverage >> min 1);
    LevelValueOption.Ratio (unlimitedUseAreaDamageAverage >> max 1);
  }
  |> estimateLevelValueOptionAttribute averageDamage originalLevel newLevel
  |> double
  |> averageToNearestDamageDice

let estimateLimitedUseAreaDamage averageDamage originalLevel newLevel =
  seq {
    LevelValueOption.Ratio (limitedUseAreaDamageAverage >> min 1);
    LevelValueOption.Ratio (limitedUseAreaDamageAverage >> max 1);
  }
  |> estimateLevelValueOptionAttribute averageDamage originalLevel newLevel
  |> double
  |> averageToNearestDamageDice

let estimateRegeneration regeneration originalLevel newLevel =
  seq {
    LevelValueOption.Ratio (moderateRegeneration >> min 1);
    LevelValueOption.Interval (moderateRegeneration, highRegeneration);
    LevelValueOption.Ratio (highRegeneration >> max 1);
  }
  |> estimateLevelValueOptionAttribute regeneration originalLevel newLevel

