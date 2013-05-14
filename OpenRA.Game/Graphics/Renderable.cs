#region Copyright & License Information
/*
 * Copyright 2007-2013 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using System.Collections.Generic;
using System.Drawing;

namespace OpenRA.Graphics
{
	public class RenderableComparer : IComparer<Renderable>
	{
		WorldRenderer wr;
		public RenderableComparer(WorldRenderer wr)
		{
			this.wr = wr;
		}

		public int Compare(Renderable x, Renderable y)
		{
			return x.SortOrder(wr).CompareTo(y.SortOrder(wr));
		}
	}

	public struct Renderable
	{
		readonly Sprite Sprite;
		readonly WPos Pos;
		readonly float Scale;

		// TODO: Fix Parachute and WithShadow so these can be made private
		public readonly PaletteReference Palette;
		public readonly int ZOffset;

		public Renderable(Sprite sprite, WPos pos, int zOffset, PaletteReference palette, float scale)
		{
			Sprite = sprite;
			Pos = pos;
			Palette = palette;
			ZOffset = zOffset;
			Scale = scale;
		}

		public Renderable(Sprite sprite, float2 pos, PaletteReference palette, int z, int zOffset, float scale)
			: this(sprite, new PPos((int)pos.X, (int)pos.Y).ToWPos(0), z + zOffset, palette, scale) { }

		public Renderable(Sprite sprite, float2 pos, PaletteReference palette, int z)
			: this(sprite, pos, palette, z, 0, 1f) { }

		public Renderable(Sprite sprite, float2 pos, PaletteReference palette, int z, float scale)
			: this(sprite, pos, palette, z, 0, scale) { }

		public Renderable WithScale(float newScale) { return new Renderable(Sprite, Pos, ZOffset, Palette, newScale); }
		public Renderable WithPalette(PaletteReference newPalette) { return new Renderable(Sprite, Pos, ZOffset, newPalette, Scale); }
		public Renderable WithZOffset(int newOffset) { return new Renderable(Sprite, Pos, newOffset, Palette, Scale); }
		public Renderable WithPos(WPos pos) { return new Renderable(Sprite, pos, ZOffset, Palette, Scale); }

		// Transitional hack
		public Renderable WithPxOffset(float2 offset)
		{
			var x = (int)(Pos.X * Game.CellSize / 1024 + offset.X);
			var y = (int)(Pos.Y * Game.CellSize / 1024 + offset.Y);
			var z = (int)(Pos.Z * Game.CellSize / 1024);
			return new Renderable(Sprite, new PPos(x,y).ToWPos(z), ZOffset, Palette, Scale);
		}

		public void Render(WorldRenderer wr)
		{
			Sprite.DrawAt(wr.ScreenPxPosition(Pos) - 0.5f*Scale*Sprite.size, Palette.Index, Scale);
		}

		public Size Size
		{
			get
			{
				var size = (Scale*Sprite.size).ToInt2();
				return new Size(size.X, size.Y);
			}
		}

		public int SortOrder(WorldRenderer wr)
		{ 
			return (int)wr.ScreenZPosition(Pos) + ZOffset;
		}
	}
}
