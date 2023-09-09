using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismArcPack
{
	class Program
	{
		static void PackFixOutOfMemory(string inputfolder, string outputfile)
		{
			var filenames = new List<string>();
			var offsets = new List<int>();
			var sizes = new List<int>();

			var position = 0;
			foreach (var f in Directory.GetFiles(inputfolder))
			{
				var fname = Path.GetFileName(f);
				filenames.Add(fname);
				offsets.Add(position);

				var b = File.ReadAllBytes(f);
				position += b.Length;
				sizes.Add(b.Length);
			}


			if (File.Exists(outputfile))
			{
				File.Delete(outputfile);
			}
			var ms = new FileStream(outputfile, FileMode.OpenOrCreate);
			var br = new BinaryWriter(ms);
			
			br.Write("GAMEDAT PAC2".ToCharArray());
			br.Write(filenames.Count);

			foreach (var fname in filenames)
			{
				br.Write(fname.ToCharArray());
				for(var i = 0; i < 32 - fname.Length; i++)
					br.Write('\0');
			}

			for (var i = 0; i < offsets.Count; i++)
			{
				br.Write(offsets[i]);
				br.Write(sizes[i]);
			}

			ms.Close();

			var datams = new FileStream(outputfile, FileMode.Append);
			var databr = new BinaryWriter(datams);

			foreach (var f in Directory.GetFiles(inputfolder))
			{
				var b = File.ReadAllBytes(f);
				databr.Write(b);
				databr.Flush();
			}


			datams.Close();
		}

		static void Pack(string inputfolder, string outputfile)
		{
			var filenames = new List<string>();
			var offsets = new List<int>();
			var sizes = new List<int>();

			var datams = new MemoryStream();
			var databr = new BinaryWriter(datams);

			foreach (var f in Directory.GetFiles(inputfolder))
			{
				var fname = Path.GetFileName(f);
				filenames.Add(fname);
				offsets.Add((int)datams.Position);

				var b = File.ReadAllBytes(f);

				sizes.Add(b.Length);

				databr.Write(b);
			}

			var ms = new MemoryStream();
			var br = new BinaryWriter(ms);

			br.Write("GAMEDAT PAC2".ToCharArray());
			br.Write(filenames.Count);

			foreach (var fname in filenames)
			{
				br.Write(fname.ToCharArray());
				for (var i = 0; i < 32 - fname.Length; i++)
					br.Write('\0');
			}

			for (var i = 0; i < offsets.Count; i++)
			{
				br.Write(offsets[i]);
				br.Write(sizes[i]);
			}

			br.Write(datams.ToArray());

			File.WriteAllBytes(outputfile, ms.ToArray());
		}


		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine("Usage: PrismArcPack (Source Directory) (Destination Archive)");
				return;
			}
			PackFixOutOfMemory(args[0], args[1]);
			// PackFixOutOfMemory(@"E:\Visual Novel Pack\Taisho x Alice\TaiAli PC\Episode 2\2 - Images\unpacked", @"E:\Visual Novel Pack\Taisho x Alice\TaiAli PC\Episode 2\2 - Images\graphic_patched.dat");
			// Pack(@"E:\Visual Novel Pack\Taisho x Alice\TaiAli PC\Episode 2\2 - Images\test", @"E:\Visual Novel Pack\Taisho x Alice\TaiAli PC\Episode 2\2 - Images\graphic_test.dat");
		}
	}
}
