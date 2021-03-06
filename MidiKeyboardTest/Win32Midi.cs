﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MidiKeyboardTest
{
    class Win32Midi
    {
        /// <summary>
        /// UINT midiInGetNumDevs(VOID);
        /// </summary>
        /// <returns>
        /// 関数が成功すると、システムに存在する MIDI 入力デバイスの数が返ります。戻り値が 0 の場合は、デバイスが存在しないことを示します(エラーがないという意味ではありません）。
        /// </returns>
        [DllImport("winmm.dll", EntryPoint = "midiInGetNumDevs")]
        public static extern uint MidiInGetNumDevs();

        /// <summary>
        /// MMRESULT midiInGetDevCaps(
        ///     UINT uDeviceID,
        ///     LPMIDIINCAPS lpMidiInCaps,
        ///     UINT cbMidiInCaps
        /// );
        /// </summary>
        /// <param name="deviceId">MIDI 入力デバイスの識別子を指定します。デバイス識別子の値は 0 から存在するデバイス数未満（存在するデバイス数から 1 を引いた数）までの範囲です。また、このパラメータには、適切にキャストされたデバイスハンドルを指定してもかまいません。</param>
        /// <param name="midiInCaps">デバイスの性能に関する情報が入る MIDIINCAPS 構造体のアドレスを指定します。</param>
        /// <param name="cbMidiInCaps">MIDIINCAPS 構造体のサイズをバイト単位で指定します。cbMidiInCaps バイト以下の情報だけが、lpMidiInCaps パラメータで指定した場所にコピーされます。cbMidiInCaps パラメータが 0 の場合は何もコピーされず、関数は MMSYSERR_NOERROR を返します。</param>
        /// <returns>
        /// 関数が成功すると、MMSYSERR_NOERROR が返ります。関数が失敗すると、エラーが返ります。返されるエラー値は次のとおりです。
        /// MMSYSERR_BADDEVICEID	指定されたデバイス識別子は範囲外です。
        /// MMSYSERR_INVALPARAM	指定されたポインタまたは構造体は無効です。
        /// MMSYSERR_NODRIVER	ドライバがインストールされていません。
        /// MMSYSERR_NOMEM	システムはメモリを割り当てられないか、またはロックできません。
        /// </returns>
        [DllImport("winmm.dll", EntryPoint = "midiInGetDevCaps")]
        public static extern uint MidiInGetDevCaps(uint deviceId, ref MidiInCaps midiInCaps, uint cbMidiInCaps);

        /// <summary>
        /// typedef struct {
        ///     WORD      wMid;
        ///     WORD      wPid;
        ///     MMVERSION vDriverVersion;
        ///     TCHAR     szPname[MAXPNAMELEN];
        ///     DWORD     dwSupport;
        /// } MIDIINCAPS;
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MidiInCaps
        {
            /// <summary>
            /// Manufacturer identifier of the device driver for the MIDI input device. Manufacturer identifiers are defined in Manufacturer and Product Identifiers.
            /// </summary>
            public UInt16 mid;

            /// <summary>
            /// Product identifier of the MIDI input device. Product identifiers are defined in Manufacturer and Product Identifiers.
            /// </summary>
            public UInt16 pid;

            /// <summary>
            /// Version number of the device driver for the MIDI input device. The high-order byte is the major version number, and the low-order byte is the minor version number.
            /// </summary>
            public uint mmversion;

            /// <summary>
            /// Product name in a null-terminated string.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPNameLen)]
            public string pname;

            /// <summary>
            /// Reserved; must be zero.
            /// </summary>
            public uint support;
        }

        /// <summary>
        /// #define MAXPNAMELEN      32     /* max product name length (including NULL) */
        /// </summary>
        public const int MaxPNameLen = 32;

        /// <summary>
        /// MMRESULT midiInOpen(
        ///     LPHMIDIIN lphMidiIn,
        ///     UINT uDeviceID,
        ///     DWORD dwCallback,
        ///     DWORD dwCallbackInstance,
        ///     DWORD dwFlags
        /// );
        /// </summary>
        /// <param name="midiIn">HMIDIIN ハンドルのアドレスを指定します。この場所には、オープンした MIDI 入力デバイスを識別するハンドルが入ります。ハンドルは、ほかの MIDI 入力関数の呼び出しでこのデバイスを識別するために使われます。</param>
        /// <param name="deviceId">オープンする MIDI 入力デバイスの識別子を指定します。</param>
        /// <param name="callback">着信 MIDI メッセージに関する情報によって呼び出されるコールバック関数、スレッド識別子、またはウィンドウのハンドルのアドレスを指定します。コールバック関数の詳細については、MidiInProc を参照してください。</param>
        /// <param name="callbackInstance">コールバック関数に渡されるユーザーインスタンスデータを指定します。このパラメータは、ウィンドウコールバック関数またはスレッドとともには使われません。</param>
        /// <param name="flags">
        /// デバイスをオープンするためのコールバックフラグと、高速データ転送を調節するステータスフラグ(任意）を指定します。次の値のいずれかになります。
        /// CALLBACK_FUNCTION
        /// dwCallback パラメータはコールバックプロシージャアドレスです。
        /// CALLBACK_NULL
        /// コールバック機構はありません。この値が既定の設定です。
        /// CALLBACK_THREAD
        /// dwCallback パラメータはスレッド識別子です。
        /// CALLBACK_WINDOW
        /// dwCallback パラメータはウィンドウハンドルです。
        /// MIDI_IO_STATUS
        /// このパラメータに CALLBACK_FUNCTION も指定する場合は、MIM_DATA メッセージとともに MIM_MOREDATA メッセージがコールバック関数に送信されます。このパラメータに CALLBACK_WINDOW も指定する場合は、MM_MIM_DATA メッセージとともに MM_MIM_MOREDATA メッセージがウィンドウに送信されます。このフラグはイベントコールバックまたはスレッドコールバックには影響しません。
        /// コールバック機構を使うほとんどのアプリケーションでは、このパラメータに CALLBACK_FUNCTION を指定します。
        /// </param>
        /// <returns>
        /// 関数が成功すると、MMSYSERR_NOERROR が返ります。関数が失敗すると、エラーが返ります。返されるエラー値は次のとおりです。
        /// MMSYSERR_ALLOCATED	指定されたリソースは既に割り当てられています。
        /// MMSYSERR_BADDEVICEID	指定されたデバイス識別子は範囲外です。
        /// MMSYSERR_INVALFLAG	dwFlags パラメータで指定されたフラグは無効です。
        /// MMSYSERR_INVALPARAM	指定されたポインタまたは構造体は無効です。
        /// MMSYSERR_NOMEM	システムはメモリを割り当てられないか、またはロックできません。
        /// </returns>
        [DllImport("winmm.dll", EntryPoint = "midiInOpen")]
        public static extern uint MidiInOpen(ref IntPtr midiIn, uint deviceId, IntPtr callback, IntPtr callbackInstance, uint flags);

        public delegate void MidiInOpenCallback(IntPtr hdrvr, uint msg, uint user, uint dw1, uint dw2);

        /// <summary>
        /// #define CALLBACK_FUNCTION   0x00030000l    /* dwCallback is a FARPROC */
        /// </summary>
        public const uint CallbackFunction = 0x00030001;

        /// <summary>
        /// MMRESULT midiInClose(
        ///   HMIDIIN hMidiIn 
        /// );
        /// </summary>
        /// <param name="midiIn">MIDI 入力デバイスのハンドルを指定します。関数が成功すると、このハンドルは無効になります。</param>
        /// <returns>
        /// 関数が成功すると、MMSYSERR_NOERROR が返ります。関数が失敗すると、エラーが返ります。返されるエラー値は次のとおりです。
        /// MIDIERR_STILLPLAYING	バッファはまだキューにあります。
        /// MMSYSERR_INVALHANDLE	指定されたデバイスハンドルは無効です。
        /// MMSYSERR_NOMEM	システムはメモリを割り当てられないか、またはロックできません。
        /// </returns>
        [DllImport("winmm.dll", EntryPoint = "midiInClose")]
        public static extern uint midiInClose(ref IntPtr midiIn);

        public const uint MMSysErrNoError = 0;

        /// <summary>
        /// MMRESULT midiInGetErrorText(
        ///   MMRESULT wError,
        ///   LPSTR lpText,
        ///   UINT cchText
        /// );
        /// </summary>
        /// <param name="error">エラーコードを指定します。</param>
        /// <param name="text">エラーの原文記述が入るバッファのアドレスを指定します。</param>
        /// <param name="cchText">lpText パラメータで指定したバッファの長さを文字数で指定します。</param>
        /// <returns>関数が成功すると、MMSYSERR_NOERROR が返ります。関数が失敗すると、エラーが返ります。返されるエラー値は次のとおりです。</returns>
        [DllImport("winmm.dll", EntryPoint = "midiInGetErrorText")]
        public static extern uint MidiInGetErrorText(uint error, StringBuilder text, uint cchText);

        /// <summary>
        /// MMRESULT midiInStart(
        ///   HMIDIIN hMidiIn 
        /// );
        /// </summary>
        /// <param name="midiIn">MIDI 入力デバイスのハンドルを指定します。</param>
        /// <returns>
        /// 関数が成功すると、MMSYSERR_NOERROR が返ります。関数が失敗すると、エラーが返ります。返されるエラー値は次のとおりです。
        /// </returns>
        [DllImport("winmm.dll", EntryPoint = "midiInStart")]
        public static extern uint MidiInStart(IntPtr midiIn);

        /// <summary>
        /// MMRESULT midiInStop(
        ///   HMIDIIN hMidiIn 
        /// );
        /// </summary>
        /// <param name="midiIn">MIDI 入力デバイスのハンドルを指定します。</param>
        /// <returns>
        /// 関数が成功すると、MMSYSERR_NOERROR が返ります。関数が失敗すると、エラーが返ります。返されるエラー値は次のとおりです。
        /// </returns>
        [DllImport("winmm.dll", EntryPoint = "midiInStop")]
        public static extern uint MidiInStop(IntPtr midiIn);
    }
}
