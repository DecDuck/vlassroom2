using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace server.Classes
{
	[Serializable]
	public class Transform
	{
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;

		public Transform()
		{
			position = new Vector3(0, 0, 0);
			rotation = new Vector3(0, 0, 0);
			scale = new Vector3(0, 0, 0);
		}
	}
	[Serializable]
	public class Vector3
	{
		public float x;
		public float y;
		public float z;

		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public float[] toFloatArray()
		{
			float[] array = new float[3];
			array[0] = x;
			array[1] = y;
			array[2] = z;
			return array;
		}
		public static Vector3 fromFloatArray(float[] array)
		{
			return new Vector3(array[0], array[1], array[2]);
		}
	}
}
