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
				GoDirectionByPreference(directions);
			else
				MoveInFavoriteDirection(directions[0].Direction, directions[1].Direction, directions[2].Direction, directions[3].Direction);

			base.Move();
		}

		private float CalculateDistanceToPacman(Direction d)
        {
			return DistanceBetweenNode(GameState.Map.GetNode(X, Y).GetNeighbour(d), GameState.Pacman.Node);
		}

		private float DistanceBetweenNode(Node n1, Node n2)
        {
			return (float)Math.Sqrt(Math.Pow(n1.X - n2.X, 2) + Math.Pow(n1.Y - n2.Y, 2));
		}

		private void GoDirectionByPreference(DirectionDistance[] directionsByPref)
        {
			foreach (DirectionDistance d in directionsByPref)
				if (PossibleDirections().Contains(d.Direction))
                {
					NextDirection = d.Direction;
					return;
                }
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

	public class DirectionDistance
	{
		Direction _direction;
		float _distance;

		public DirectionDistance(Direction direction, float distance)
        {
			_direction = direction;
			_distance = distance;
        }

		public Direction Direction
        {
			get { return _direction; }
        }

		public float Distance
        {
			get { return _distance; }
        }
	}
}
