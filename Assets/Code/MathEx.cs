namespace Code {

	public static class MathEx {
		public static void Swap<T>(ref T a, ref T b) => (a, b) = (b, a);
	}

}