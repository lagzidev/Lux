using System;

namespace LuxEngine
{
    public class MoveCommand : Command
    {
        private Character _character;
        private MoveDirection _direction;
        private MoveSpeed _speed;

        public MoveCommand(Character character, MoveDirection direction, MoveSpeed speed)
        {
            _character = character;
            _direction = direction;
            _speed = speed;
        }

        public override void Execute()
        {
            _character.Move(_direction, _speed);
        }
    }
}
