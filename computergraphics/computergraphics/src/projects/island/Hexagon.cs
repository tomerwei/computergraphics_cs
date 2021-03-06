﻿using System;
using OpenTK;

namespace computergraphics
{
	public class Hexagon
	{
		private Vector3 pos;
		private Hexagon[] neighbors = { null, null, null, null, null, null };
		private float sideLength = 0.5f;
		private static float[] N_ANGLES = { ToRadiens(0), ToRadiens(60), ToRadiens(120), ToRadiens(180), ToRadiens(240), ToRadiens(300) };
		private static float[] V_ANGLES = { ToRadiens(30), ToRadiens(90), ToRadiens(150), ToRadiens(210), ToRadiens(270), ToRadiens(330) };

		public float SideLength
		{
			get { return sideLength; }
		}

		public Hexagon(Vector3 pos, float sideLength)
		{
			this.pos = pos;
			this.sideLength = sideLength;
		}

		/**
		 * Generate the neighbor hexagon at the given index (0 = top, clockwise, valid indices: 0...5)
		 */
		public Hexagon CreateNeighbor(int nIndex)
		{
			float d = 2.0f * (float)(sideLength / 2.0f * Math.Sqrt(3.0f));
			Vector3 nPos = new Vector3(pos.X + (float)Math.Sin(N_ANGLES[nIndex]) * d,
									   pos.Y + (float)Math.Cos(N_ANGLES[nIndex]) * d,
									   pos.Z);
			Hexagon h = new Hexagon(nPos, sideLength);
			SetNeighbor(nIndex, h);
			return neighbors[nIndex];
		}

		/**
		 * Sets the neighbor fields in both directions and checks for adjacent connections.
		 * */
		private void SetNeighbor(int nIndex, Hexagon h)
		{
			SetNeighborBidirectional(nIndex, h);

			// Clockwise
			int nPlus = (nIndex + 1) % 6;
			Hexagon hNext = neighbors[nPlus];
			while (hNext != null)
			{
				int idx = (GetOppositeIndex(nPlus) + 1) % 6;
				hNext.SetNeighborBidirectional(idx, h);

				nPlus = (idx + 1) % 6;
				hNext = hNext.neighbors[nPlus];
			}

			// Counter-clockwise
			nPlus = (nIndex + 5) % 6;
			hNext = neighbors[nPlus];
			while (hNext != null)
			{
				int idx = (GetOppositeIndex(nPlus) + 5) % 6;
				hNext.SetNeighborBidirectional(idx, h);

				nPlus = (idx + 5) % 6;
				hNext = hNext.neighbors[nPlus];
			}
		}

		/**
		 * Sets the value of the nieghbor field in both directions
		 * */
		private void SetNeighborBidirectional(int nIndex, Hexagon h)
		{
			if (neighbors[nIndex] != null)
			{
				Console.WriteLine("SetNeighborBidirectional(): Inconsistency detected!");
			}
			if (h.neighbors[GetOppositeIndex(nIndex)] != null)
			{
				Console.WriteLine("SetNeighborBidirectional(): Inconsistency detected!");
			}

			neighbors[nIndex] = h;
			h.neighbors[GetOppositeIndex(nIndex)] = this;
		}

		/**
		 * Get the opposite neighbor index
		 **/
		public static int GetOppositeIndex(int index)
		{
			return (index + 3) % 6;
		}

		public Vector3 GetCorner(int index )
		{
			return new Vector3(pos.X + (float)Math.Sin(V_ANGLES[index]) * sideLength,
							   pos.Y + (float)Math.Cos(V_ANGLES[index]) * sideLength,
							   pos.Z);
		}

		/**
         * Generate i'the texture coordinate. Size is the width of the texture footprint (0...1). center is the
         * center of the texture foodprint must be >size/2 and smaller 1-size/2
         */
		public static Vector2 getTexCoord(float size, Vector2 center, int index)
		{
			return new Vector2(center.X + (float)Math.Sin(V_ANGLES[index]) * size,
							   center.Y + (float)Math.Cos(V_ANGLES[index]) * size);
		}

		/**
		 * Generate a triagle mesh for the hexagon
		 */
		public ITriangleMesh GenerateMesh()
		{
			ITriangleMesh mesh = new TriangleMesh();
			Vector2[] texCoords = new Vector2[7];
			Vector2 texCoordCenter = new Vector2(0.2f, 0.2f);
			for (int i = 0; i < 6; i++)
			{
				Vector3 v = GetCorner(i);
				mesh.AddVertex(v);
				texCoords[i] = getTexCoord(0.2f, texCoordCenter, i);
			}
			mesh.AddVertex(pos);
			texCoords[6] = texCoordCenter;

			int texCoordStartIndex = mesh.GetNumberOfTexCoords();
			for (int i = 0; i < 7; i++)
			{
				mesh.AddTextureCoordinate(texCoords[i]);
			}

			for (int i = 0; i < 6; i++)
			{
				Triangle t = new Triangle(i, 6, (i + 1) % 6, texCoordStartIndex+i, texCoordStartIndex + 6, texCoordStartIndex + (i + 1) % 6);
				mesh.AddTriangle(t);
			}

			mesh.ComputeTriangleNormals();
			return mesh;
		}

		/**
		 * Convert degree -> radiens
		 * */
		private static float ToRadiens(float degree)
		{
			return (float)(degree * Math.PI / 180.0);
		}

		/**
		 * Returns true if the neighborhood datastructure is consistent
		 **/
		public bool ConsistencyCheck()
		{
			bool consistent = true;
			for (int i = 0; i < 6; i++)
			{
				if (neighbors[i] != null)
				{
					consistent = consistent && neighbors[i].neighbors[GetOppositeIndex(i)] == this;
				}

			}
			return consistent;
		}

		/**
		 * Returns true, if there is at least one open border.
		 * */
		public bool IsBorder()
		{
			for (int i = 0; i < 6; i++)
			{
				if (neighbors[i] == null)
				{
					return true;
				}
			}
			return false;
		}

		/**
		 * Returns a random border index. Only valid if HasBorder() gives true.
		 * */
		public int GetRandomBorderIndex()
		{
			for (int i = 0; i < 6; i++)
			{
				if (neighbors[i] == null)
				{
					return i;
				}
			}
			return -1;
		}

		public Hexagon GetNeighbor(int index)
		{
			return neighbors[index];
		}
	}
}
