using System;
using System.Runtime.InteropServices;

namespace WindowsDesktop.Interop
{
	[StructLayout(LayoutKind.Sequential)]
	public struct HString : IDisposable
	{
		private readonly IntPtr handle;
		public static HString FromString(string s)
		{
			var h = Marshal.AllocHGlobal(IntPtr.Size);
			Marshal.ThrowExceptionForHR(WindowsCreateString(s, s.Length, h));
			return Marshal.PtrToStructure<HString>(h);
		}

		public void Delete()
		{
			WindowsDeleteString(handle);
		}

		[DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall)]
		private static extern int WindowsCreateString(
			[MarshalAs(UnmanagedType.LPWStr)] string sourceString,
			int length,
			[Out] IntPtr hstring
		);

		[DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
		private static extern int WindowsDeleteString(IntPtr hstring);

		[DllImport("api-ms-win-core-winrt-string-l1-1-0.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, CharSet = CharSet.Unicode)]
		private static extern IntPtr WindowsGetStringRawBuffer(HString hString, IntPtr length);

		public void Dispose()
		{
			Delete();
		}

		public static implicit operator string(HString hString)
		{
			var str = Marshal.PtrToStringUni(WindowsGetStringRawBuffer(hString, IntPtr.Zero));
			hString.Delete();
			return str ?? string.Empty;
		}

	}

}
