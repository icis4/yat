//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2004 Mike Krüger.
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;

/// <summary>
/// This namespace declares the native libusb elements.
/// Elements aren't documented for consistency/maintenance reasons. See libusb for documentation.
/// </summary>
namespace libusb.NET.Native
{
	/// <summary>
	/// Wraps all required native functions of libusb.
	/// </summary>
	/// <remarks>
	/// All original prototypes of the functions are included.
	/// However, only functions currently required are implemented.
	/// </remarks>
	public static class Functions
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

	#if WIN32 || WIN64
		public const int PathMax = 512;
	#else // Linux
		public const int PathMax = 4096 + 1;
	#endif

	#if WIN32
		#if !DEBUG
		    private const string LIBUSB_DLL = "libusb-1.0_Win32.dll";
		#else
			private const string LIBUSB_DLL = "libusb-1.0_Win32_Debug.dll";
		#endif
	#elif WIN64
		#if !DEBUG
		    private const string LIBUSB_DLL = "libusb-1.0_x64.dll";
		#else
		    private const string LIBUSB_DLL = "libusb-1.0_x64_Debug.dll";
		#endif
	#else // Linux
	    	private const string LIBUSB_DLL = "libusb";
	#endif

		private const CallingConvention CALLING_CONVENTION = CallingConvention.Cdecl;

		#endregion

		#region Static Lifetime
		//==========================================================================================
		// Static Lifetime
		//==========================================================================================

		static Functions()
		{
			libusb_init();
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Methods > libusb.h/core.c
		//------------------------------------------------------------------------------------------
		// Methods > libusb.h/core.c
		//------------------------------------------------------------------------------------------

		// ssize_t libusb_get_device_list(libusb_context *ctx, libusb_device ***list);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_get_device_list(IntPtr ctx, out libusb_device[] list);
		public  static        int libusb_get_device_list(out libusb_device[] list)
		{
			return (libusb_get_device_list(IntPtr.Zero, out list));
		}

		// void libusb_free_device_list(libusb_device **list, int unref_devices);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern void libusb_free_device_list(out libusb_device[] list, int unref_devices);
		public  static        void libusb_free_device_list(out libusb_device[] list, bool unref_devices)
		{
			libusb_free_device_list(out list, Convert.ToInt32(unref_devices));
		}

		// uint8_t libusb_get_bus_number(libusb_device *dev);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern byte libusb_get_bus_number(libusb_device dev);

		// uint8_t libusb_get_device_address(libusb_device *dev);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern byte libusb_get_device_address(libusb_device dev);

		// int libusb_get_max_packet_size(libusb_device *dev, unsigned char endpoint);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_get_max_packet_size(libusb_device dev, byte endpoint);

		// int libusb_get_max_iso_packet_size(libusb_device *dev, unsigned char endpoint);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_get_max_iso_packet_size(libusb_device dev, byte endpoint);
		
		// libusb_device *libusb_ref_device(libusb_device *dev);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern libusb_device libusb_ref_device(libusb_device dev);
		
		// void libusb_unref_device(libusb_device *dev);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern void libusb_unref_device(libusb_device dev);
		
		// int libusb_open(libusb_device *dev, libusb_device_handle **handle);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_open(libusb_device dev, out libusb_device_handle handle);
		
		// libusb_device_handle *libusb_open_device_with_vid_pid(libusb_context *ctx, uint16_t vendor_id, uint16_t product_id);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern libusb_device_handle libusb_open_device_with_vid_pid(IntPtr ctx, ushort vendor_id, ushort product_id);
		public  static        libusb_device_handle libusb_open_device_with_vid_pid(ushort vendor_id, ushort product_id)
		{
			return (libusb_open_device_with_vid_pid(IntPtr.Zero, vendor_id, product_id));
		}
		
		// void libusb_close(libusb_device_handle *dev_handle);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern void libusb_close(libusb_device_handle dev_handle);
		
		// libusb_device *libusb_get_device(libusb_device_handle *dev_handle);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern libusb_device libusb_get_device(libusb_device_handle dev_handle);
		
		// int libusb_get_configuration(libusb_device_handle *dev, int *config);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_get_configuration(libusb_device_handle dev, out int config);
		
		// int libusb_set_configuration(libusb_device_handle *dev, int configuration);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_set_configuration(libusb_device_handle dev, int configuration);
		
		// int libusb_claim_interface(libusb_device_handle *dev, int interface_number);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_claim_interface(libusb_device_handle dev, int interface_number);
		
		// int libusb_release_interface(libusb_device_handle *dev, int interface_number);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_release_interface(libusb_device_handle dev, int interface_number);
		
		// int libusb_set_interface_alt_setting(libusb_device_handle *dev, int interface_number, int alternate_setting);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_set_interface_alt_setting(libusb_device_handle dev, int interface_number, int alternate_setting);
		
		// int libusb_clear_halt(libusb_device_handle *dev, unsigned char endpoint);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_clear_halt(libusb_device_handle dev, byte endpoint);
		
		// int libusb_reset_device(libusb_device_handle *dev);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_reset_device(libusb_device_handle dev);
		
		// int libusb_kernel_driver_active(libusb_device_handle *dev, int interface_number);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_kernel_driver_active(libusb_device_handle dev, int interface_number);
		
		// int libusb_detach_kernel_driver(libusb_device_handle *dev, int interface_number);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_detach_kernel_driver(libusb_device_handle dev, int interface_number);
		
		// int libusb_attach_kernel_driver(libusb_device_handle *dev, int interface_number);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_attach_kernel_driver(libusb_device_handle dev, int interface_number);
		
		// void libusb_set_debug(libusb_context *ctx, int level);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public  static extern void libusb_set_debug(IntPtr ctx, int level);
		private static        void libusb_set_debug(int level)
		{
			libusb_set_debug(IntPtr.Zero, level);
		}
		
		// int libusb_init(libusb_context **context);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_init(IntPtr ctx);
		public  static        int libusb_init()
		{
			return (libusb_init(IntPtr.Zero));
		}
		
		// void libusb_exit(struct libusb_context *ctx);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern void libusb_exit(IntPtr ctx);
		public  static        void libusb_exit()
		{
			libusb_exit(IntPtr.Zero);
		}
		
		// const char* libusb_strerror(enum libusb_error errcode);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern string libusb_strerror(libusb_error errcode);

		#endregion

		#region Methods > libusb.h/descriptor.c
		//------------------------------------------------------------------------------------------
		// Methods > libusb.h/descriptor.c
		//------------------------------------------------------------------------------------------

		// int libusb_get_device_descriptor(libusb_device *dev, struct libusb_device_descriptor *desc);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_get_device_descriptor(libusb_device dev, ref libusb_device_descriptor desc);

		// int libusb_get_active_config_descriptor(libusb_device *dev, struct libusb_config_descriptor **config);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_get_active_config_descriptor(libusb_device dev, out libusb_config_descriptor config);

		// int libusb_get_config_descriptor(libusb_device *dev, uint8_t config_index, struct libusb_config_descriptor **config);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_get_config_descriptor(libusb_device dev, byte config_index, out libusb_config_descriptor config);

		// int libusb_get_config_descriptor_by_value(libusb_device *dev, uint8_t bConfigurationValue, struct libusb_config_descriptor **config);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_get_config_descriptor_by_value(libusb_device dev, byte bConfigurationValue, out libusb_config_descriptor config);

		// void libusb_free_config_descriptor(struct libusb_config_descriptor *config);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern void libusb_free_config_descriptor(libusb_config_descriptor config);

		// int libusb_get_string_descriptor_ascii(libusb_device_handle *dev, uint8_t desc_index, unsigned char *data, int length);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_get_string_descriptor_ascii(libusb_device_handle dev, byte desc_index, out byte[] data, int length);
		public  static        int libusb_get_string_descriptor_ascii(libusb_device_handle dev, byte desc_index, out string data)
		{
			byte[] ascii = new byte[Constants.MaximumStringDescriptorByteLength];
			int ret = libusb_get_string_descriptor_ascii(dev, desc_index, out ascii, ascii.Length);

			StringBuilder sb = new StringBuilder();
			foreach (byte b in ascii)
			{
				if (b != 0)
					sb.Append(b);
				else
					break;
			}
			data = sb.ToString();

			return (ret);
		}

		#endregion

		#region Methods > libusb.h/io.c
		//------------------------------------------------------------------------------------------
		// Methods > libusb.h/io.c
		//------------------------------------------------------------------------------------------

		// struct libusb_transfer *libusb_alloc_transfer(int iso_packets);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern libusb_transfer libusb_alloc_transfer(int iso_packets);

		// void libusb_free_transfer(struct libusb_transfer *transfer);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern void libusb_free_transfer(libusb_transfer transfer);

		// int libusb_submit_transfer(struct libusb_transfer *transfer);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_submit_transfer(libusb_transfer transfer);

		// int libusb_cancel_transfer(struct libusb_transfer *transfer);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		public static extern int libusb_cancel_transfer(libusb_transfer transfer);

		// int libusb_try_lock_events(libusb_context *ctx);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_try_lock_events(IntPtr ctx);
		public  static        int libusb_try_lock_events()
		{
			return (libusb_try_lock_events(IntPtr.Zero));
		}

		// void libusb_lock_events(libusb_context *ctx);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern void libusb_lock_events(IntPtr ctx);
		public  static        void libusb_lock_events()
		{
			libusb_lock_events(IntPtr.Zero);
		}

		// void libusb_unlock_events(libusb_context *ctx);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern void libusb_unlock_events(IntPtr ctx);
		public  static        void libusb_unlock_events()
		{
			libusb_unlock_events(IntPtr.Zero);
		}

		// int libusb_event_handling_ok(libusb_context *ctx);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_event_handling_ok(IntPtr ctx);
		public  static        int libusb_event_handling_ok()
		{
			return (libusb_event_handling_ok(IntPtr.Zero));
		}

		// int libusb_event_handler_active(libusb_context *ctx);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_event_handler_active(IntPtr ctx);
		public  static        int libusb_event_handler_active()
		{
			return (libusb_event_handler_active(IntPtr.Zero));
		}

		// void libusb_lock_event_waiters(libusb_context *ctx);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern void libusb_lock_event_waiters(IntPtr ctx);
		public  static        void libusb_lock_event_waiters()
		{
			libusb_lock_event_waiters(IntPtr.Zero);
		}

		// void libusb_unlock_event_waiters(libusb_context *ctx);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern void libusb_unlock_event_waiters(IntPtr ctx);
		public  static        void libusb_unlock_event_waiters()
		{
			libusb_unlock_event_waiters(IntPtr.Zero);
		}

		// int libusb_wait_for_event(libusb_context *ctx, struct timeval *tv);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_wait_for_event(IntPtr ctx, libusb_transfer transfer);
		public  static        int libusb_wait_for_event(libusb_transfer transfer)
		{
			return (libusb_wait_for_event(IntPtr.Zero, transfer));
		}

		// int libusb_handle_events_timeout(libusb_context *ctx, struct timeval *tv);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_handle_events_timeout(IntPtr ctx, libusb_transfer transfer);
		public  static        int libusb_handle_events_timeout(libusb_transfer transfer)
		{
			return (libusb_handle_events_timeout(IntPtr.Zero, transfer));
		}

		// int libusb_handle_events(libusb_context *ctx);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_handle_events(IntPtr ctx);
		public  static        int libusb_handle_events()
		{
			return (libusb_handle_events(IntPtr.Zero));
		}

		// int libusb_handle_events_locked(libusb_context *ctx, struct timeval *tv);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_handle_events_locked(IntPtr ctx, libusb_transfer transfer);
		public  static        int libusb_handle_events_locked(libusb_transfer transfer)
		{
			return (libusb_handle_events_locked(IntPtr.Zero, transfer));
		}

		// int libusb_pollfds_handle_timeouts(libusb_context *ctx);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_pollfds_handle_timeouts(IntPtr ctx);
		public  static        int libusb_pollfds_handle_timeouts()
		{
			return (libusb_pollfds_handle_timeouts(IntPtr.Zero));
		}

		// int libusb_get_next_timeout(libusb_context *ctx, struct timeval *tv);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_get_next_timeout(IntPtr ctx, libusb_transfer transfer);
		public  static        int libusb_get_next_timeout(libusb_transfer transfer)
		{
			return (libusb_get_next_timeout(IntPtr.Zero, transfer));
		}

		// void libusb_set_pollfd_notifiers(libusb_context *ctx, libusb_pollfd_added_cb added_cb, libusb_pollfd_removed_cb removed_cb, void *user_data);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern void libusb_set_pollfd_notifiers(IntPtr ctx, libusb_pollfd_added_cb added_cb, libusb_pollfd_removed_cb removed_cb, out object user_data);
		public  static        void libusb_set_pollfd_notifiers(libusb_pollfd_added_cb added_cb, libusb_pollfd_removed_cb removed_cb, out object user_data)
		{
			libusb_set_pollfd_notifiers(IntPtr.Zero, added_cb, removed_cb, out user_data);
		}

		// const struct libusb_pollfd **libusb_get_pollfds(libusb_context *ctx);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern libusb_pollfd[] libusb_get_pollfds(IntPtr ctx);
		public  static        libusb_pollfd[] libusb_get_pollfds()
		{
			return (libusb_get_pollfds(IntPtr.Zero));
		}

		#endregion

		#region Methods > libusb.h/sync.c
		//------------------------------------------------------------------------------------------
		// Methods > libusb.h/sync.c
		//------------------------------------------------------------------------------------------

		// int libusb_control_transfer(libusb_device_handle *dev_handle, uint8_t bmRequestType, uint8_t bRequest, uint16_t wValue, uint16_t wIndex, unsigned char *data, uint16_t wLength, unsigned int timeout);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_control_transfer(libusb_device_handle dev_handle, byte bmRequestType, byte bRequest, ushort wValue, ushort wIndex, ref byte[] data, ushort wLength, uint timeout);
		public  static        int libusb_control_transfer(libusb_device_handle dev_handle, byte bmRequestType, byte bRequest, ushort wValue, ushort wIndex, ref byte[] data, int timeout)
		{
			return (libusb_control_transfer(dev_handle, bmRequestType, bRequest, wValue, wIndex, ref data, Convert.ToUInt16(data != null ? data.Length : 0), Convert.ToUInt32(timeout)));
		}

		// int libusb_bulk_transfer(struct libusb_device_handle *dev_handle, unsigned char endpoint, unsigned char *data, int length, int *transferred, unsigned int timeout);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_bulk_transfer(libusb_device_handle dev_handle, byte endpoint, ref byte[] data, int length, out int transferred, uint timeout);
		public  static        int libusb_bulk_transfer(libusb_device_handle dev_handle, byte endpoint, ref byte[] data, out int transferred, int timeout)
		{
			return (libusb_bulk_transfer(dev_handle, endpoint, ref data, (data != null ? data.Length : 0), out transferred, Convert.ToUInt32(timeout)));
		}

		// int libusb_interrupt_transfer(struct libusb_device_handle *dev_handle, unsigned char endpoint, unsigned char *data, int length, int *transferred, unsigned int timeout);
		[DllImport(LIBUSB_DLL, CallingConvention = CALLING_CONVENTION, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
		private static extern int libusb_interrupt_transfer(libusb_device_handle dev_handle, byte endpoint, ref byte[] data, int length, out int transferred, uint timeout);
		public  static        int libusb_interrupt_transfer(libusb_device_handle dev_handle, byte endpoint, ref byte[] data, out int transferred, int timeout)
		{
			return (libusb_interrupt_transfer(dev_handle, endpoint, ref data, (data != null ? data.Length : 0), out transferred, Convert.ToUInt32(timeout)));
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
