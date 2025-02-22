module FileLoader.Regex

open System.Text.RegularExpressions
// https://regex101.com/

let compiledRegex regex = 
  Regex(regex, RegexOptions.Compiled)

let damageRegex =
  compiledRegex("(?<={@damage )\d*(d|D)\d*((\+|\-)\d*)?")

let matchDamage text = 
  damageRegex.Match(text)
  |> string
  
let matchesDamage text =
  damageRegex.Matches(text)
  |> Seq.map string
