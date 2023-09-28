Azalea provides multiple ways to get user input:

If you need game wide input like mouse position or hovered objects, you can use the static class [[Input Class]].

If you need [[GameObject]] specific input you can either:
1. Override the On*Action* methods within that object. (NOTE: For the meaning of the bool return value for some actions refer to [[Consuming Input]].
2. Subscribe to the *Action* events from outside the object.
3. Use action specific objects like [[Button]].