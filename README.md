# pathfinder-2e-scaler-api
An API service that can help GMs scale Pathfinder creatures up and down

This is just an api server so it doesn't have any UI

- List of creatures: just their names and levels
  - https://pathfinder-2e-scaler-api-zcygi53jbq-uc.a.run.app/
- The creature's regular stats can be found by visiting the `/{creature-name}` page, in this case, a level 2 boar
  - https://pathfinder-2e-scaler-api-zcygi53jbq-uc.a.run.app/Boar   
- The creature's scaled up stats `/{creature-name}/{level}`, a level 20 boar
  - https://pathfinder-2e-scaler-api-zcygi53jbq-uc.a.run.app/Boar/20

I'm only extracting the values that can change at the moment.

Currently only using bestiaries 1-3 from: https://github.com/Pf2eToolsOrg/Pf2eTools/tree/master/data/bestiary

This is done with the extra project in the `file-import` branch. I've excluded it from `main` due to limited dev ops knowledge and it not being necessary to run the services.

See issues for known bugs.
