using System;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace WondevWomanUI
{
	class MainClass : Form
	{
		private PictureBox box;
		public static void Main (string[] args)
		{
			MainClass form = new MainClass ();
			form.ShowDialog ();
		}

		static int size;
		static int unitsPerPlayer;
		public MainClass() {
			box = new PictureBox { Parent = this, Dock = DockStyle.Fill };

			size = int.Parse (Console.ReadLine ());
			unitsPerPlayer = int.Parse (Console.ReadLine ());
			scale = (xMax - xMin) / size;
			unmarkedBoard = (Bitmap)Image.FromStream (Assembly.GetExecutingAssembly ().GetManifestResourceStream ("background.jpg"));
			this.Width = 10 + unmarkedBoard.Width;
			this.Height = 10 + unmarkedBoard.Height;
			ReadState ();
			box.Click += MakeMovement;
		}

		int xMin = 512, xMax = 1407, yMin = 165, scale;
		List<Point> chain = new List<Point>();
		private void MakeMovement(object sender, EventArgs e) {
			Point cursor = box.PointToClient (Cursor.Position);
			int gridX = (cursor.X - xMin) / scale;
			int gridY = (cursor.Y - yMin) / scale;
			if (gridX < 0 || gridX >= size || gridY < 0 || gridY >= size) {
				box.Image.Dispose ();
				box.Image = unmarkedBoard;
				chain.Clear ();
				return;
			}
			chain.Add (new Point (gridX, gridY));
			Image bmp = new Bitmap (unmarkedBoard);
			Graphics g = Graphics.FromImage (bmp);
			foreach(Point p in chain)
				g.DrawRectangle (Pens.BlueViolet, xMin + scale * p.X, yMin + scale * p.Y, scale, scale);
			box.Image = bmp;
			g.Dispose ();
			if (chain.Count == 3) {
				int index = myPos.IndexOf (chain [0]);
				string dir1 = ("N S" [Math.Sign (chain [1].Y - chain [0].Y) + 1].ToString () + "W E" [Math.Sign (chain [1].X - chain [0].X) + 1]).Trim ();
				string dir2 = ("N S" [Math.Sign (chain [2].Y - chain [1].Y) + 1].ToString () + "W E" [Math.Sign (chain [2].X - chain [1].X) + 1]).Trim ();
				string command = " " + index + " " + dir1 + " " + dir2;
				if (actions.Any (a => a.EndsWith (command)))
					command = actions.First (a => a.EndsWith (command));
				else
					command = null;
				chain.Clear ();
				if (command != null) {
					Console.WriteLine (command);
					ReadState ();
				}
			}
		}

		List<Point> myPos = new List<Point> ();
		List<string> actions = new List<string>();
		private Bitmap unmarkedBoard;
		public void ReadState() {
			string[] inputs;
			Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			unmarkedBoard = (Bitmap)Image.FromStream (Assembly.GetExecutingAssembly ().GetManifestResourceStream ("background.jpg"));
			Bitmap bmp = unmarkedBoard;
			Graphics g = Graphics.FromImage (bmp);
			for (int y = 0; y < size; y++)
			{
				string row = Console.ReadLine();
				for (int x = 0; x < size; x++) {
					if (row [x] >= '0' && row [x] <= '4') {
						for (int c = '0'; c <= row [x]; c++) {							
							g.DrawImage (Image.FromStream (assembly.GetManifestResourceStream ("tile" + (c + 1 - '0') + ".png")), xMin + scale * x, yMin + scale * y, scale, scale);
						}
					}
				}
			}
			myPos.Clear ();
			for (int i = 0; i < unitsPerPlayer; i++)
			{
				inputs = Console.ReadLine().Split(' ');
				int unitX = int.Parse(inputs[0]);
				int unitY = int.Parse(inputs[1]);
				myPos.Add (new Point (unitX, unitY));
				g.DrawImage (Image.FromStream (assembly.GetManifestResourceStream ("player1.png")), xMin + scale * unitX, yMin + scale * unitY, scale, scale);
			}
			for (int i = 0; i < unitsPerPlayer; i++) {
				inputs = Console.ReadLine ().Split (' ');
				int otherX = int.Parse (inputs [0]);
				int otherY = int.Parse (inputs [1]);
				if (otherX != -1)
					g.DrawImage (Image.FromStream (assembly.GetManifestResourceStream ("player2.png")), xMin + scale * otherX, yMin + scale * otherY, scale, scale);
			}
			int legalActions = int.Parse(Console.ReadLine());
			if (legalActions == 0)
				Console.WriteLine ("ACCEPT-DEFEAT");
			actions.Clear ();
			for (int i = 0; i < legalActions; i++)
			{
				actions.Add (Console.ReadLine ());
			}

			g.Dispose ();
			if (box.Image != null)
				box.Image.Dispose ();
			box.Image = bmp;
			unmarkedBoard = bmp;
		}
	}
}
