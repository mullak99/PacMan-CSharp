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

		bool useCustomMove = true;
		Direction[] possibleDirections = new Direction[] { Direction.Up, Direction.Right, Direction.Down, Direction.Left };
		public override void Move() {
			DirectionDistance[] directions = new DirectionDistance[4];

			for (int i = 0; i < possibleDirections.Length; i++)
			{
				Direction d = possibleDirections[i];
				directions[i] = new DirectionDistance(d, CalculateDistanceToPacman(d));
			}
			directions = directions.OrderBy(x => x.Distance).ToArray();

			/* 
			 * GoDirectionByPreference causes the ghost to have 'bouncy movement' when it gets in a corner. This could be solved, but 
			 * it wouldnt really be a Moving Target Search since the fastest path would need to be ignored for a few movement cycles.
			 */
			if (useCustomMove)
				GoDirectionByPreference(directions, true);
			else
				MoveInFavoriteDirection(directions[0].Direction, directions[1].Direction, directions[2].Direction, directions[3].Direction);

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
