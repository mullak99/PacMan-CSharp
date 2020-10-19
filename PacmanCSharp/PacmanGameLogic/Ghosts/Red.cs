using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

namespace Pacman.GameLogic.Ghosts
{
    [Serializable()]
	public class Red : Ghost, ICloneable
	{
		public const int StartX = 111, StartY = 93;

		public Red(int x, int y, GameState gameState, double Speed, double FleeSpeed)
			: base(x, y, gameState) {
			this.name = "Red";
			ResetPosition();
            this.Speed = Speed;
            this.FleeSpeed = FleeSpeed;
        }

		public override void PacmanDead() {
			waitToEnter = 0;
			ResetPosition();			
		}

		public override void ResetPosition() {
			x = StartX;
			y = StartY;
			waitToEnter = 0;
			direction = Direction.Left;
			base.ResetPosition();
			entered = true;
		}

		const bool MitigateBouncyMovement = true;
		public override void Move() {
			DirectionDistance[] directions = new DirectionDistance[PossibleDirections().Count];
			
			for (int i = 0; i < PossibleDirections().Count; i++)
			{
				Direction d = PossibleDirections()[i];
				directions[i] = new DirectionDistance(d, CalculateDistanceToPacman(d));
			}

			Array.Sort(directions);
			GoDirectionByPreference(directions, MitigateBouncyMovement);

			base.Move();
		}

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public new Red Clone()
        {
            Red _temp = (Red)this.MemberwiseClone();
            _temp.Node = Node.Clone();

            return _temp;
        }

        #endregion
    }
}
