﻿using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace computergraphics
{
	/**
	 * Translates all child node along a given vector.
	 * */
	public class TranslationNode : GroupNode
	{

		private Vector3 translation;

		public TranslationNode (Vector3 translation)
		{
			this.translation = translation;
		}

		public override void Draw()
		{
			GL.PushMatrix ();
			GL.MatrixMode(MatrixMode.Modelview);
			GL.Translate (translation);
			base.Draw ();
			GL.PopMatrix ();
		}
	}
}

