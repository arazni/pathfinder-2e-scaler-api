module Scaler.Estimations

open Scaler.Gamemastery

type NearDamage = {
  count: int
  size: int
  bonus: decimal
}

let add x y = x + y
let times x y = x * y

type AbsoluteCompare<'a> = int -> 'a -> ('a -> int) -> int
type IntervalCompare<'a> = int -> 'a -> ('a -> int) -> ('a -> int) -> double
type AbsoluteLevelValue = (int -> int)
type IntervalLevelValue = (int -> int) * (int -> int)

type ComparerOption<'a> =
  | Absolute of AbsoluteCompare<'a>
  | Interval of IntervalCompare<'a>

type LevelValueOption =
  | Absolute of (int -> int)
  | Interval of (int -> int) * (int -> int)

let absoluteAttributeCompare attribute originalLevel estimator =
  abs ((estimator originalLevel) - attribute)

let absoluteAttributeSort attribute originalLevel estimators =
  Seq.sortBy (fun estimator -> absoluteAttributeCompare attribute originalLevel estimator) estimators

let intervalRatio value originalLevel estimatorLow estimatorHigh =
  let (low, high) = (absoluteAttributeCompare value originalLevel estimatorLow, absoluteAttributeCompare value originalLevel estimatorHigh)
  (double low) / double (low + high)

let estimateAbsoluteAttributeFromClosestFunction attribute originalLevel newLevel closestFunction =
  attribute - closestFunction originalLevel + closestFunction newLevel

let estimateIntervalAttributeFromClosestFunction attribute originalLevel newLevel (fLow: int -> int) (fHigh: int -> int) =
  let ratio = intervalRatio attribute originalLevel fLow fHigh
  (ratio * double (fHigh newLevel - fLow newLevel)) 
  |> round
  |> int
  |> add (fLow newLevel)

let estimateAbsoluteAttribute attribute originalLevel newLevel functions =
  absoluteAttributeSort attribute originalLevel functions |> Seq.head
  |> estimateAbsoluteAttributeFromClosestFunction attribute originalLevel newLevel

let intervalCompare value originalLevel estimatorLow estimatorHigh =
  //if value <= estimatorLow originalLevel then double (absoluteAttributeCompare value originalLevel estimatorLow)
  //else if value >= estimatorHigh originalLevel then double (absoluteAttributeCompare value originalLevel estimatorHigh)
  //else intervalRatio value originalLevel estimatorLow estimatorHigh
  if value <= estimatorLow originalLevel || value >= estimatorHigh originalLevel then None
  else intervalRatio value originalLevel estimatorLow estimatorHigh |> Some

let intervalSortValue attribute originalLevel levelValueOption =
  match levelValueOption with
  | Interval (fLow, fHigh) -> intervalCompare attribute originalLevel fLow fHigh
  | Absolute f -> 
    absoluteAttributeCompare attribute originalLevel f
    |> double
    |> Some

let closestMixedAttributeComparer attribute originalLevel comparers =
  Seq.filter (fun comparer -> (intervalSortValue attribute originalLevel comparer) |> Option.isSome) comparers
  |> Seq.sortBy (fun comparer -> double (intervalSortValue attribute originalLevel comparer).Value)
  |> Seq.head

let estimateLevelValueOption attribute originalLevel newLevel levelValueOption =
  match levelValueOption with
  | Interval (low, high) -> estimateIntervalAttributeFromClosestFunction attribute originalLevel newLevel low high
  | Absolute estimator -> estimateAbsoluteAttributeFromClosestFunction (int attribute) originalLevel newLevel estimator

let estimateLevelValueOptionAttribute attribute originalLevel newLevel levelValueOptions =
  levelValueOptions
  |> closestMixedAttributeComparer attribute originalLevel
  |> estimateLevelValueOption attribute originalLevel newLevel

let estimatePerception (perception: int) (originalLevel: int) (newLevel: int) =
  seq { terriblePerception; lowPerception; moderatePerception; highPerception; extremePerception }
  |> estimateAbsoluteAttribute perception originalLevel newLevel

let estimateAbilityModifier modifier originalLevel newLevel =
  if originalLevel < 1 then seq { lowAbilityModifier; moderateAbilityModifier; highAbilityModifier; } else seq { lowAbilityModifier; moderateAbilityModifier; highAbilityModifier; extremeAbilityModifier }
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
let diceSizeToAverage size = decimal (size+1) / 2m
let possibleDiceAverages = Seq.map diceSizeToAverage possibleDiceSizes
let toDecimal seq = Seq.map (fun x -> decimal x) seq

let averageToMatchingDamageDiceSize (average: decimal) (size: int) =
  { 
    bonus = average % (diceSizeToAverage size);
    count = int (average / (diceSizeToAverage size));
    size = size
  }

let private _averageToPossibleDamageDice (average: decimal) sizes =
  Seq.map (fun size -> averageToMatchingDamageDiceSize average size) sizes
  |> Seq.where (fun possible -> possible.count > 0)

let averageToPossibleDamageDice (average: decimal) sizes =
  match average with
  | average when average < 3m -> seq { averageToMatchingDamageDiceSize average defaultDiceSize }
  | _ -> _averageToPossibleDamageDice average sizes

let averageToNearestDamageDice (average: decimal) sizes =
  averageToPossibleDamageDice average sizes
  |> Seq.sortBy (fun nearby -> nearby.bonus)
  |> Seq.take 1

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

let estimateStrikeDamage averageDamage dieSize originalLevel newLevel =
  let sizes = seq {}
  {
    count = strikeDamageDiceCount newLevel,
    size = 
  }

// strike damage, area damage