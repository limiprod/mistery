using System;
using System.Runtime.InteropServices;

namespace ControllerManager
{
    [StructLayout(LayoutKind.Sequential)]
    public struct XINPUT_STATE
    {
        public uint PacketNumber;
        public XINPUT_GAMEPAD Gamepad;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XINPUT_GAMEPAD
    {
        public ushort wButtons;
        public byte bLeftTrigger;
        public byte bRightTrigger;
        public short sThumbLX;
        public short sThumbLY;
        public short sThumbRX;
        public short sThumbRY;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XINPUT_VIBRATION
    {
        public ushort wLeftMotorSpeed;
        public ushort wRightMotorSpeed;
    }

    public static class XInputController
    {
        // Máscaras dos botões principais
        public const ushort BotaoA = 0x1000;
        public const ushort BotaoB = 0x2000;
        public const ushort BotaoX = 0x4000;
        public const ushort BotaoY = 0x8000;
        public const ushort DPadUp = 0x0001;
        public const ushort DPadDown = 0x0002;
        public const ushort DPadLeft = 0x0004;
        public const ushort DPadRight = 0x0008;

        [DllImport("xinput1_4.dll", EntryPoint = "XInputGetState")]
        private static extern uint XInputGetState(uint dwUserIndex, ref XINPUT_STATE pState);

        [DllImport("xinput1_4.dll", EntryPoint = "XInputSetState")]
        private static extern uint XInputSetState(uint dwUserIndex, ref XINPUT_VIBRATION pVibration);

        /// <summary>
        /// Verifica se o comando está ligado na porta especificada (ex: 0).
        /// </summary>
        public static bool EstaConectado(uint playerIndex = 0)
        {
            XINPUT_STATE state = new XINPUT_STATE();
            uint resultado = XInputGetState(playerIndex, ref state);
            return resultado == 0;
        }

        /// <summary>
        /// Verifica se um determinado botão está a ser pressionado.
        /// </summary>
        public static bool EstaPremido(ushort mascaraBotao, uint playerIndex = 0)
        {
            XINPUT_STATE state = new XINPUT_STATE();
            if (XInputGetState(playerIndex, ref state) == 0)
            {
                return (state.Gamepad.wButtons & mascaraBotao) != 0;
            }
            return false;
        }

        /// <summary>
        /// Define a vibração do comando. Os valores variam de 0 a 65535.
        /// </summary>
        public static void Vibrar(ushort velocidadeEsquerda, ushort velocidadeDireita, uint playerIndex = 0)
        {
            XINPUT_VIBRATION vibracao = new XINPUT_VIBRATION();
            vibracao.wLeftMotorSpeed = velocidadeEsquerda;
            vibracao.wRightMotorSpeed = velocidadeDireita;
            XInputSetState(playerIndex, ref vibracao);
        }

        /// <summary>
        /// Desliga completamente a vibração do comando.
        /// </summary>
        public static void PararVibracao(uint playerIndex = 0)
        {
            Vibrar(0, 0, playerIndex);
        }
    }
}