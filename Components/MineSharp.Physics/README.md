## MineSharp.Physics

Logic to simulate a Minecraft Player in a minecraft world.
The following things are supported:
 - Gravity
 - Walking in all directions
 - Sprinting
 - Crouching (Can't crouch under a slap yet)
 - Water current pushing (Still a bit buggy)

The following things should be implemented at some point:
 - Swimming
 - Elytra?
 - Riding vehicles
 - Climbing must be tested

The code follows the same checks the vanilla minecraft client does. \
Only client side checks are done.

### Example
```csharp
    var input = new InputControls();
    var physics = new PlayerPhysics(data, player, world, input);
    physics.Tick(); // Calculates movement for one single tick.
                    // Updates the position and velocity of the player's entity object.
                    // The vanilla client calls this every 50ms
    
    input.ForwardKeyDown = true; // Start walking forward
    input.BackwardKeyDown = true; // Start walking backwards
    input.LeftKeyDown = true; // Start walking to the left
    input.RightKeyDown = true; // Start walking to the right
    input.JumpingKeyDown = true; // Start jumping (also used for swimming when implemented)
    input.SneakingKeyDown = true; // Start sneaking
    input.SprintingKeyDown = true; // Start sprinting
```