# EvoSim

Simulation of evolution

Contains:
- Genetic algorithm
- Neural network
- Evolution simulation

To start, set the Evolution project as the StartUp project.

When running, several stationary and mobile dots can be seen, some with a triangular arc protruding from it:
- White dots are "food"
- Blue dots are "dangers"
- All other dots are "blobs". These range in colour from green to red, depending on their health. The triangular arc is the area of the board that they can see.

Each of the blobs have a unique neural network controlling their movements, as well as unique genetics determining their neural network and other characteristics. Eating food will increase their health and eating dangers will decrease it. In addition, each iteration will decrease their health by a tiny amount. These blobs move around and, eventually, die from starvation, unless they eat frequently enough. After running the simulation for around 20000 - 40000 iterations, some blobs should have emerged that are more successfull at eating food, thereby surviving longer. After eating a certain amount of food, they will split, creating a slightly mutated copy of themselves. Blobs that have been created this way can be identified by the number in brackets after their id - this is their parent's id. Exceptionally successful blobs may even meet and produce offspring. This offspring will be a combination of both parents, again with some mutations. By default, such pairing of blobs is hard and will normally not occur before the 150.000 - 200.000 iteration mark.

Interesting blobs can be saved by clicking export, and can later be imported again by clicking import. In addition, many parameters are available:
- BaseEyeSightLength: determines the base distance the blobs can see, on average. Their actual sight length is also influenced by their genetics
- BaseEyeSightWidth: determines the base width of their sight, on average.
- BlobCount: the amount of blobs that need to be present at all times. If a blob dies of starvation, and the amount of blobs would fall below this level, a random blob is added.
- BlobSpeed: determines the base speed at which blobs can move.
- DangerCount: the amount of dangers present at any time.
- DangerValue: how much health is lost upon touching a danger.
- EatenCountForMate: the amount of food that needs to have been eaten for a blob to be eligible for mating
- EatenCountForSplit: the amount of food that needs to have been eaten for a blob to be eligible for splitting
- EntitiesToPass: the amount of entities (food, danger, other blobs) to the neural network of each blob per iteration. The closest entities are always passed first.
- EntityRadius: the radius of the entities, in pixels.
- FoodCount: the amount of food present at any time.
- FoodValue: the amount of health that is regenerated upon eating food.
- Health: the maximum amount of health a blob can have.
- MateDamage: the damage done to a blob when mating.
- MateFrom: the minimum amount of health needed for a blob to be able to mate (as a factor of Health, between 0 and 1).
- MaxBlobs: the maximum amount of blobs that may be present. If a new blob is added and the MaxBlobs parameter would be passed, the weakest blob dies.
- MaxEyeSightLength: the maximum distance any blob can see.
- MaxEyeSightWidth: the maximum width of their sight.
- MaxTurnPerTick: the maximum amount of degrees any blob may turn per iteration.
- StepDamage: the amount of damage done to the blobs per iteration.
- TimerStep: the minimum time between two iterations, in ms.