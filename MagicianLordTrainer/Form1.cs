/**
*  File           Form1.cs
*  Brief          Magician Lord (GOG Version) PC Trainer for use with version: setup_magician_lord_1.0_(69161).exe
*  Copyright      2025 Shawn M. Crawford [sleepy]
*  Date           12/16/2025
*
*  Author         Shawn M. Crawford [sleepy]
*
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MagicianLordTrainer
{
    public partial class Form1 : Form
    {

        #region Imports

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesRead);

        #endregion

        #region Enums

        [Flags]
        private enum ProcessAccessRights
        {
            PROCESS_CREATE_PROCESS = 0x0080,
            PROCESS_CREATE_THREAD = 0x0002,
            PROCESS_DUP_HANDLE = 0x0040,
            PROCESS_QUERY_INFORMATION = 0x0400,
            PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
            PROCESS_SET_INFORMATION = 0x0200,
            PROCESS_SET_QUOTA = 0x0100,
            PROCESS_SUSPEND_RESUME = 0x0800,
            PROCESS_TERMINATE = 0x0001,
            PROCESS_VM_OPERATION = 0x0008,
            PROCESS_VM_READ = 0x0010,
            PROCESS_VM_WRITE = 0x0020,
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            SYNCHRONIZE = 0x00100000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
            STANDARD_RIGHTS_REQUIRED = 0x000f0000,
            PROCESS_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF)
        }

        #endregion

        #region Constants

        private const string MODULE_NAME = "NeoGeo.exe"; // MAGICIAN LORD
        private const string PROCESS_NAME = "NeoGeo";

        private const int BASE_ADDRESS_0 = 0x002D8B5C; // "NeoGeo.exe"+002D8B5C
        private const int BASE_ADDRESS_1 = 0x002D8B60; // "NeoGeo.exe"+002D8B60
        private const int BASE_ADDRESS_2 = 0x002D8B8C; // "NeoGeo.exe"+002D8B8C

        // Base Address 0
        private const int OFFSET_CONTINUE_TIMER = 0x0317;

        // Base Address 1
        private const int OFFSET_SPAWN_DEATHRBINGER = 0x0064; // Spawns Death Bringer (255 = spawn)
        private const int OFFSET_HI_SCORE_1 = 0x009C; // Hi Score 110000 0
        private const int OFFSET_HI_SCORE_2 = 0x009F; // Hi Score 001100 0
        private const int OFFSET_HI_SCORE_3 = 0x009E; // Hi Score 000011 0
        private const int OFFSET_HI_SCORE_4 = 0x009D; // Hi Score Related
        private const int OFFSET_SPAWN_FLYING_MONSTERS_1 = 0x00BA; // (0 for none, 255 for many)
        private const int OFFSET_SPAWN_FLYING_MONSTERS_2 = 0x00BB; // (0 for none, 255 for many)
        private const int OFFSET_DEMO_COUNTDOWN_TIMER_1 = 0x0502; // (Each countdown adds 1 to Timer 2)
        private const int OFFSET_DEMO_COUNTDOWN_TIMER_2 = 0x0503; // (End Demo = 3)

        // Base Address 2
        private const int OFFSET_DEATHRBINGER_COUNTDOWN_TIMER = 0x01FF; // Death Bringer Countdown Timer (37, 25h)


        // Player 1
        // Base Address 1
        private const int OFFSET_P1_SCORE_1 = 0x00A0; // Score 110000 0
        private const int OFFSET_P1_SCORE_2 = 0x00A3; // Score 001100 0
        private const int OFFSET_P1_SCORE_3 = 0x00A2; // Score 000011 0
        private const int OFFSET_P1_SCORE_4 = 0x00A1; // Score Related
        
        // Base Address 2
        private const int OFFSET_P1_FORM_1 = 0x0300; // Form (0 - 6, Normal Dragon Raijin Shinobi Samurai Poseidon Waterman)
        private const int OFFSET_P1_LIVES = 0x0301;
        private const int OFFSET_P1_FORM_2 = 0x0302; // Form related 1 (10     10     12      12     12      10       10)
        private const int OFFSET_P1_FORM_3 = 0x0303; // Form related 2 (10     08     10      12     11      07       09)
        private const int OFFSET_P1_FORM_4 = 0x0304; // Orbs (0      5      15      7      11      09       10)
        private const int OFFSET_P1_POW_BAR = 0x0305;
        private const int OFFSET_P1_HP_BAR = 0x0306;
        private const int OFFSET_P1_INVINCIBILITY_TIMER = 0x0308; // Freeze at 255

        // Player 2
        // TODO

        #endregion

        #region Fields

        private Process game;

        private IntPtr hProc = IntPtr.Zero;
        private IntPtr continueTimerAddressGlobal = IntPtr.Zero;
        private IntPtr spawnDeathBringerAddressGlobal = IntPtr.Zero;
        private IntPtr hiscore1AddressGlobal = IntPtr.Zero;
        private IntPtr hiscore2AddressGlobal = IntPtr.Zero;
        private IntPtr hiscore3AddressGlobal = IntPtr.Zero;
        private IntPtr hiscore4AddressGlobal = IntPtr.Zero;
        private IntPtr spawnFlyingMonsters1AddressGlobal = IntPtr.Zero;
        private IntPtr spawnFlyingMonsters2AddressGlobal = IntPtr.Zero;
        private IntPtr demoCountdownTimer1AddressGlobal = IntPtr.Zero;
        private IntPtr demoCountdownTimer2AddressGlobal = IntPtr.Zero;
        private IntPtr deathBringerCountdownTimerAddressGlobal = IntPtr.Zero;
        private IntPtr p1Score1AddressGlobal = IntPtr.Zero;
        private IntPtr p1Score2AddressGlobal = IntPtr.Zero;
        private IntPtr p1Score3AddressGlobal = IntPtr.Zero;
        private IntPtr p1Score4AddressGlobal = IntPtr.Zero;
        private IntPtr p1LivesAddressGlobal = IntPtr.Zero;
        private IntPtr p1Form1AddressGlobal = IntPtr.Zero;
        private IntPtr p1Form2AddressGlobal = IntPtr.Zero;
        private IntPtr p1Form3AddressGlobal = IntPtr.Zero;
        private IntPtr p1Form4AddressGlobal = IntPtr.Zero;
        private IntPtr p1PowBarAddressGlobal = IntPtr.Zero;
        private IntPtr p1HpBarAddressGlobal = IntPtr.Zero;
        private IntPtr p1InvincibilitytimerAddressGlobal = IntPtr.Zero;

        private Timer timer = new Timer();

        private string previousLogEntry = "";

        #endregion

        public Form1()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            AutoScaleMode = AutoScaleMode.Dpi;
            StartPosition = FormStartPosition.CenterScreen;

            Text = "Magician Lord (GOG Version) Trainer by sLeEpY9090";
            textBoxLog.Text = "Magician Lord (GOG Version) Trainer by sLeEpY9090" + Environment.NewLine + "For use with version: setup_magician_lord_1.0_(69161).exe" + Environment.NewLine;

            SetDefaultTextBoxValues();
            SetTextBoxMaxLength();
            PopulateScore();
            PopulateFormTypes();
            PopulateLeftOrb1Types();
            PopulateRightOrb2Types();

            // Keep trainer updated about the game process and game memory.
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        #region Form Setup Methods

        public void SetDefaultTextBoxValues()
        {
            textBoxContinueTimerWrite.Text = "9";
            textBoxDeathBringerCountdownTimerWrite.Text = "255"; // 37d = 25h = Death Bringer Countdown Timer Default 
            textBoxSpawnDeathBringerWrite.Text = "0"; // 255 = Spawn DeathBringer
            textBoxFlyingMonsters1Write.Text = "0"; // 0 = None
            textBoxFlyingMonsters2Write.Text = "0"; // 0 = None
            textBoxDemoTimer1Write.Text = "255"; // Each countdown adds 1 to Timer 2
            textBoxDemoTimer2Write.Text = "0"; // 3 = End Demo
            textBoxP1HPBarWrite.Text = "4";
            textBoxP1InvincibilityTimerWrite.Text = "255";  // 255 = Invincible
            textBoxP1PowBarWrite.Text = "4";
            textBoxP1LivesWrite.Text = "9";
            textBoxP1Form2Write.Text = "10"; // Default
            textBoxP1Form3Write.Text = "10"; // Default


            textBoxContinueTimerRead.Enabled = false;
            textBoxDeathBringerCountdownTimerRead.Enabled = false;
            textBoxSpawnDeathBringerRead.Enabled = false;
            textBoxFlyingMonsters1Read.Enabled = false;
            textBoxFlyingMonsters2Read.Enabled = false;
            textBoxDemoTimer1Read.Enabled = false;
            textBoxDemoTimer2Read.Enabled = false;
            textBoxHighScoreRead.Enabled = false;
            textBoxP1HPBarRead.Enabled = false;
            textBoxP1InvincibilityTimerRead.Enabled = false;
            textBoxP1PowBarRead.Enabled = false;
            textBoxP1ScoreRead.Enabled = false;
            textBoxP1LivesRead.Enabled = false;
            textBoxP1Form2Read.Enabled = false;
            textBoxP1Form3Read.Enabled = false;

            comboBoxP1FormRead.Enabled = false;
            comboBoxFormLeftOrb1Read.Enabled = false;
            comboBoxFormRightOrb2Read.Enabled = false;

            textBoxLog.Enabled = false;

            textBoxModuleName.Text = MODULE_NAME;
            textBoxProcessName.Text = PROCESS_NAME;
        }

        private void SetTextBoxMaxLength()
        {
            textBoxContinueTimerWrite.MaxLength = 3;
            textBoxDeathBringerCountdownTimerWrite.MaxLength = 3;
            textBoxSpawnDeathBringerWrite.MaxLength = 3;
            textBoxFlyingMonsters1Write.MaxLength = 3;
            textBoxFlyingMonsters2Write.MaxLength = 3;
            textBoxDemoTimer1Write.MaxLength = 3;
            textBoxDemoTimer2Write.MaxLength = 3;

            textBoxP1HPBarWrite.MaxLength = 3;
            textBoxP1InvincibilityTimerWrite.MaxLength = 3;
            textBoxP1PowBarWrite.MaxLength = 3;
            textBoxP1LivesWrite.MaxLength = 3;
            textBoxP1Form2Write.MaxLength = 3;
            textBoxP1Form3Write.MaxLength = 3;
        }

        private void PopulateScore()
        {
            for (int i = 0; i <= 255; i++)
            {
                comboBoxP1Score1.Items.Add(i.ToString("X"));
                comboBoxP1Score2.Items.Add(i.ToString("X"));
                comboBoxP1Score3.Items.Add(i.ToString("X"));

                comboBoxHiScore1.Items.Add(i.ToString("X"));
                comboBoxHiScore2.Items.Add(i.ToString("X"));
                comboBoxHiScore3.Items.Add(i.ToString("X"));
            }

            comboBoxP1Score4.Items.Add(0);
            comboBoxHiScore4.Items.Add(0);

            comboBoxP1Score1.SelectedIndex = 153; // 153h = 99d
            comboBoxP1Score2.SelectedIndex = 153;
            comboBoxP1Score3.SelectedIndex = 153;
            comboBoxP1Score4.SelectedIndex = 0;

            comboBoxHiScore1.SelectedIndex = 153;
            comboBoxHiScore2.SelectedIndex = 153;
            comboBoxHiScore3.SelectedIndex = 153;
            comboBoxHiScore4.SelectedIndex = 0;
        }

        private void PopulateFormTypes()
        {
            Dictionary<int, string> formTypesDictionary = new Dictionary<int, string>
            {
                {  0, "0 - Normal"  },
                {  1, "1 - Dragon"   },
                {  2, "2 - Raijin"  },
                {  3, "3 - Shinobi"   },
                {  4, "4 - Samurai"  },
                {  5, "5 - Poseidon"   },
                {  6, "6 - Waterman"  }
            };

            comboBoxP1FormRead.Items.Clear();
            comboBoxP1FormRead.DataSource = new BindingSource(formTypesDictionary, null);
            comboBoxP1FormRead.DisplayMember = "Value";
            comboBoxP1FormRead.ValueMember = "Key";
            comboBoxP1FormRead.SelectedIndex = 0;

            comboBoxP1FormWrite.Items.Clear();
            comboBoxP1FormWrite.DataSource = new BindingSource(formTypesDictionary, null);
            comboBoxP1FormWrite.DisplayMember = "Value";
            comboBoxP1FormWrite.ValueMember = "Key";
            comboBoxP1FormWrite.SelectedIndex = 0;
        }

        private void PopulateLeftOrb1Types()
        {
            Dictionary<int, string> orbTypesDictionary = new Dictionary<int, string>
            {
                {  0, "None"  },
                {  4, "Fire"   },
                {  8, "Water"  },
                {  12, "Wind"   }
            };

            comboBoxFormLeftOrb1Read.Items.Clear();
            comboBoxFormLeftOrb1Read.DataSource = new BindingSource(orbTypesDictionary, null);
            comboBoxFormLeftOrb1Read.DisplayMember = "Value";
            comboBoxFormLeftOrb1Read.ValueMember = "Key";
            comboBoxFormLeftOrb1Read.SelectedIndex = 0;

            comboBoxFormLeftOrb1Write.Items.Clear();
            comboBoxFormLeftOrb1Write.DataSource = new BindingSource(orbTypesDictionary, null);
            comboBoxFormLeftOrb1Write.DisplayMember = "Value";
            comboBoxFormLeftOrb1Write.ValueMember = "Key";
            comboBoxFormLeftOrb1Write.SelectedIndex = 0;
        }

        private void PopulateRightOrb2Types()
        {
            Dictionary<int, string> orbTypesDictionary = new Dictionary<int, string>
            {
                {  0, "None"  },
                {  1, "Fire"   },
                {  2, "Water"  },
                {  3, "Wind"   }
            };

            comboBoxFormRightOrb2Read.Items.Clear();
            comboBoxFormRightOrb2Read.DataSource = new BindingSource(orbTypesDictionary, null);
            comboBoxFormRightOrb2Read.DisplayMember = "Value";
            comboBoxFormRightOrb2Read.ValueMember = "Key";
            comboBoxFormRightOrb2Read.SelectedIndex = 0;

            comboBoxFormRightOrb2Write.Items.Clear();
            comboBoxFormRightOrb2Write.DataSource = new BindingSource(orbTypesDictionary, null);
            comboBoxFormRightOrb2Write.DisplayMember = "Value";
            comboBoxFormRightOrb2Write.ValueMember = "Key";
            comboBoxFormRightOrb2Write.SelectedIndex = 0;
        }

        #endregion

        private Process GameConnect()
        {
            string processName = PROCESS_NAME;
            if (checkBoxProcessName.Checked)
            {
                processName = textBoxProcessName.Text;
            }

            // Game process: shocktro
            game = Process.GetProcessesByName(processName).FirstOrDefault();
            if (game == null)
            {
                hProc = IntPtr.Zero;
            }
            else
            {
                // Open handle with full permission to game process
                hProc = OpenProcess((int)ProcessAccessRights.PROCESS_ALL_ACCESS, false, game.Id);
            }

            return game;
        }

        private IntPtr GetBaseAddress()
        {
            string moduleName = MODULE_NAME;
            if (checkBoxModuleName.Checked)
            {
                moduleName = textBoxModuleName.Text;
            }

            IntPtr baseAddress = IntPtr.Zero;
            foreach (ProcessModule module in game.Modules)
            {
                if (module.ModuleName == moduleName)
                {
                    baseAddress = module.BaseAddress;
                    break;
                }
            }

            return baseAddress;
        }

        private IntPtr GetOffsetAddress(IntPtr hProc, int address, int offset)
        {
            IntPtr baseAddress = GetBaseAddress();
            IntPtr offsetAddress = IntPtr.Zero;
            if (baseAddress != IntPtr.Zero)
            {
                offsetAddress = IntPtr.Add(baseAddress, address);
                offsetAddress = ReadPointerUInt32(hProc, offsetAddress, offset);
            }
            return offsetAddress;
        }

        #region Display Value Methods

        private string DisplayByteValue(IntPtr hProc, IntPtr addressGlobal, int baseAddress, int valueOffset)
        {
            if (addressGlobal == IntPtr.Zero || ((int)addressGlobal) == valueOffset)
            {
                addressGlobal = GetOffsetAddress(hProc, baseAddress, valueOffset);
            }
            int tempValue = ReadByte(hProc, addressGlobal);
            return tempValue.ToString();
        }

        private string DisplayUInt16Value(IntPtr hProc, IntPtr addressGlobal, int baseAddress, int valueOffset)
        {
            if (addressGlobal == IntPtr.Zero || ((int)addressGlobal) == valueOffset)
            {
                addressGlobal = GetOffsetAddress(hProc, baseAddress, valueOffset);
            }
            uint tempValue = ReadUInt16(hProc, addressGlobal);
            return tempValue.ToString();
        }

        private string DisplayUInt32Value(IntPtr hProc, IntPtr addressGlobal, int baseAddress, int valueOffset)
        {
            if (addressGlobal == IntPtr.Zero || ((int)addressGlobal) == valueOffset)
            {
                addressGlobal = GetOffsetAddress(hProc, baseAddress, valueOffset);
            }
            uint tempValue = ReadUInt32(hProc, addressGlobal);
            return tempValue.ToString();
        }

        #endregion

        #region Get Pointer Methods

        private IntPtr ReadPointerInt64(IntPtr hProcess, IntPtr address, int offset)
        {
            byte[] buffer = new byte[8];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            IntPtr ptr = (IntPtr)BitConverter.ToInt64(buffer, 0);
            return IntPtr.Add(ptr, offset);
        }

        private IntPtr ReadPointerUInt64(IntPtr hProcess, IntPtr address, int offset)
        {
            byte[] buffer = new byte[8];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            IntPtr ptr = (IntPtr)BitConverter.ToUInt64(buffer, 0);
            return IntPtr.Add(ptr, offset);
        }

        private IntPtr ReadPointerInt32(IntPtr hProcess, IntPtr address, int offset)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            IntPtr ptr = (IntPtr)BitConverter.ToInt32(buffer, 0);
            return IntPtr.Add(ptr, offset);
        }

        private IntPtr ReadPointerUInt32(IntPtr hProcess, IntPtr address, int offset)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            IntPtr ptr = (IntPtr)BitConverter.ToUInt32(buffer, 0);
            return IntPtr.Add(ptr, offset);
        }

        private IntPtr ReadPointerInt16(IntPtr hProcess, IntPtr address, int offset)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            IntPtr ptr = (IntPtr)BitConverter.ToInt16(buffer, 0);
            return IntPtr.Add(ptr, offset);
        }

        private IntPtr ReadPointerUInt16(IntPtr hProcess, IntPtr address, int offset)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            IntPtr ptr = (IntPtr)BitConverter.ToUInt16(buffer, 0);
            return IntPtr.Add(ptr, offset);
        }

        #endregion

        #region Get/Set Memory Value Methods

        private float ReadFloat(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[4]; // FLOAT = 4 byte
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            return BitConverter.ToSingle(buffer, 0);
        }

        private void WriteFloat(IntPtr hProcess, IntPtr address, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesWritten);
        }

        private int ReadInt(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[4]; // INT = 4 byte
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        private void WriteInt(IntPtr hProcess, IntPtr address, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesWritten);
        }

        private int ReadByte(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[1];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            return buffer[0];
        }

        private void WriteByte(IntPtr hProcess, IntPtr address, byte value)
        {
            // Since bitconverter getbytes with a byte value handles it as a short (no overload for byte), we get 2 bytes back as a short, but we only want the first
            // https://learn.microsoft.com/en-us/dotnet/api/system.bitconverter.getbytes?view=net-9.0#system-bitconverter-getbytes(system-int16)
            byte[] buffer = BitConverter.GetBytes(value);
            //WriteProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesWritten);
            WriteProcessMemory(hProcess, address, buffer, buffer.Length - 1, out int bytesWritten);
        }

        private uint ReadUInt16(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            return BitConverter.ToUInt16(buffer, 0);
        }

        private void WriteUInt16(IntPtr hProcess, IntPtr address, ushort value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesWritten);
        }

        private uint ReadUInt32(IntPtr hProcess, IntPtr address)
        {
            byte[] buffer = new byte[4];
            ReadProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesRead);
            return BitConverter.ToUInt32(buffer, 0);
        }

        private void WriteUInt32(IntPtr hProcess, IntPtr address, uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            WriteProcessMemory(hProcess, address, buffer, buffer.Length, out int bytesWritten);
        }

        #endregion

        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((GameConnect() == null) || (GetBaseAddress() == IntPtr.Zero))
            {
                ForeColor = Color.Red;
                string logEntry = "Magician Lord Process Not Found.";
                logEntry += Environment.NewLine;
                if (logEntry != previousLogEntry)
                {
                    textBoxLog.AppendText(logEntry + Environment.NewLine);
                    previousLogEntry = logEntry;
                }
            }
            else
            {
                ForeColor = Color.Green;
                string logEntry = $"[Magician Lord Process {game.ProcessName} found in {game} with PID: {game.Id}]";
                logEntry += Environment.NewLine;
                logEntry += $"Start Time: {game.StartTime}";

                if (logEntry != previousLogEntry)
                {
                    previousLogEntry = logEntry;

                    textBoxLog.AppendText(logEntry + Environment.NewLine);
                    textBoxLog.AppendText($"Total Processor Time: {game.TotalProcessorTime}"
                        + Environment.NewLine);
                    textBoxLog.AppendText($"Physical Memory Usage (MB): {game.WorkingSet64 / (1024 * 1024)}"
                        + Environment.NewLine
                        + "---------------------------------------------------"
                        + Environment.NewLine);
                }

                continueTimerAddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_0, OFFSET_CONTINUE_TIMER);

                spawnDeathBringerAddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_SPAWN_DEATHRBINGER);
                hiscore1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_HI_SCORE_1);
                hiscore2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_HI_SCORE_2);
                hiscore3AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_HI_SCORE_3);
                hiscore4AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_HI_SCORE_4);
                spawnFlyingMonsters1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_SPAWN_FLYING_MONSTERS_1);
                spawnFlyingMonsters2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_SPAWN_FLYING_MONSTERS_2);
                demoCountdownTimer1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_DEMO_COUNTDOWN_TIMER_1);
                demoCountdownTimer2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_DEMO_COUNTDOWN_TIMER_2);
                p1Score1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_SCORE_1);
                p1Score2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_SCORE_2);
                p1Score3AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_SCORE_3);
                p1Score4AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_1, OFFSET_P1_SCORE_4);

                deathBringerCountdownTimerAddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_DEATHRBINGER_COUNTDOWN_TIMER);
                p1LivesAddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P1_LIVES);
                p1Form1AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P1_FORM_1);
                p1Form2AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P1_FORM_2);
                p1Form3AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P1_FORM_3);
                p1Form4AddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P1_FORM_4);
                p1PowBarAddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P1_POW_BAR);
                p1HpBarAddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P1_HP_BAR);
                p1InvincibilitytimerAddressGlobal = GetOffsetAddress(hProc, BASE_ADDRESS_2, OFFSET_P1_INVINCIBILITY_TIMER);

                #region Level Timer

                if (checkBoxContinueTimer.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxContinueTimerWrite.Text, out int continueTimer)
                            && (continueTimer >= 0 && continueTimer <= 255))
                        {
                            WriteByte(hProc, continueTimerAddressGlobal, Convert.ToByte(continueTimer));
                        }
                        else
                        {
                            textBoxLog.AppendText("Continue Timer value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Continue Timer value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxContinueTimerRead.Text = DisplayByteValue(hProc, continueTimerAddressGlobal, BASE_ADDRESS_1, OFFSET_CONTINUE_TIMER);

                #endregion

                #region DeathBringer Timer

                if (checkBoxDeathBringerCountdownTimer.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxDeathBringerCountdownTimerWrite.Text, out int deathBringerCountdownTimer)
                            && (deathBringerCountdownTimer >= 0 && deathBringerCountdownTimer <= 255))
                        {
                            WriteByte(hProc, deathBringerCountdownTimerAddressGlobal, Convert.ToByte(deathBringerCountdownTimer));
                        }
                        else
                        {
                            textBoxLog.AppendText("DeathBringer Countdown Timer value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("DeathBringer Countdown Timer value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxDeathBringerCountdownTimerRead.Text = DisplayByteValue(hProc, deathBringerCountdownTimerAddressGlobal, BASE_ADDRESS_2, OFFSET_DEATHRBINGER_COUNTDOWN_TIMER);

                #endregion

                #region Hi-Score

                if (checkBoxHiScore.Checked)
                {
                    int hiscore1 = comboBoxHiScore1.SelectedIndex; // Score 001100 0
                    int hiscore2 = comboBoxHiScore2.SelectedIndex; // Score 001100 0
                    int hiscore3 = comboBoxHiScore3.SelectedIndex; // Score 000011 0
                    //int hiscore4 = comboBoxHiScore4.SelectedIndex; // Score Related

                    WriteByte(hProc, hiscore1AddressGlobal, (byte)hiscore1);
                    WriteByte(hProc, hiscore2AddressGlobal, (byte)hiscore2);
                    WriteByte(hProc, hiscore3AddressGlobal, (byte)hiscore3);
                    //WriteByte(hProc, highscore4AddressGlobal, (byte)hiscore4);
                }

                int tempHiScoreValue2 = ReadByte(hProc, hiscore1AddressGlobal);
                textBoxHighScoreRead.Text = tempHiScoreValue2.ToString("X").PadLeft(2, '0');
                int tempHiScoreValue = ReadByte(hProc, hiscore2AddressGlobal);
                textBoxHighScoreRead.Text += tempHiScoreValue.ToString("X").PadLeft(2, '0');
                int tempHiScoreValue4 = ReadByte(hProc, hiscore3AddressGlobal);
                textBoxHighScoreRead.Text += tempHiScoreValue4.ToString("X").PadLeft(2, '0');
                int tempHiScoreValue3 = ReadByte(hProc, hiscore4AddressGlobal);
                textBoxHighScoreRead.Text += tempHiScoreValue3.ToString("X"); //.PadLeft(2, '0');

                #endregion

                #region Score

                if (checkBoxP1Score.Checked)
                {
                    int score1 = comboBoxP1Score1.SelectedIndex; // Score 110000 0
                    int score2 = comboBoxP1Score2.SelectedIndex; // Score 001100 0
                    int score3 = comboBoxP1Score3.SelectedIndex; // Score 000011 0
                    //int score4 = comboBoxP1Score4.SelectedIndex; // Score Related

                    WriteByte(hProc, p1Score1AddressGlobal, (byte)score1);
                    WriteByte(hProc, p1Score2AddressGlobal, (byte)score2);
                    WriteByte(hProc, p1Score3AddressGlobal, (byte)score3);
                    //WriteByte(hProc, p1Score4AddressGlobal, (byte)score4);
                }

                int tempValue2 = ReadByte(hProc, p1Score1AddressGlobal);
                textBoxP1ScoreRead.Text = tempValue2.ToString("X").PadLeft(2, '0');
                int tempValue = ReadByte(hProc, p1Score2AddressGlobal);
                textBoxP1ScoreRead.Text += tempValue.ToString("X").PadLeft(2, '0');
                int tempValue4 = ReadByte(hProc, p1Score3AddressGlobal);
                textBoxP1ScoreRead.Text += tempValue4.ToString("X").PadLeft(2, '0');
                int tempValue3 = ReadByte(hProc, p1Score4AddressGlobal);
                textBoxP1ScoreRead.Text += tempValue3.ToString("X"); //.PadLeft(2, '0');

                #endregion

                #region DeathBringer

                if (checkBoxSpawnDeathBringer.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxSpawnDeathBringerWrite.Text, out int spawnDeathBringer)
                            && (spawnDeathBringer >= 0 && spawnDeathBringer <= 255))
                        {
                            WriteByte(hProc, spawnDeathBringerAddressGlobal, Convert.ToByte(spawnDeathBringer));
                        }
                        else
                        {
                            textBoxLog.AppendText("DeathBringer value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("DeathBringer value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxSpawnDeathBringerRead.Text = DisplayByteValue(hProc, spawnDeathBringerAddressGlobal, BASE_ADDRESS_1, OFFSET_SPAWN_DEATHRBINGER);

                #endregion

                #region Flying Monsters

                if (checkBoxFlyingMonsters1.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxFlyingMonsters1Write.Text, out int flyingMonsters1)
                            && (flyingMonsters1 >= 0 && flyingMonsters1 <= 255))
                        {
                            WriteByte(hProc, spawnFlyingMonsters1AddressGlobal, Convert.ToByte(flyingMonsters1));
                        }
                        else
                        {
                            textBoxLog.AppendText("Flying Monsters 1 value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Flying Monsters 1 value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxFlyingMonsters1Read.Text = DisplayByteValue(hProc, spawnFlyingMonsters1AddressGlobal, BASE_ADDRESS_1, OFFSET_SPAWN_FLYING_MONSTERS_1);

                if (checkBoxFlyingMonsters2.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxFlyingMonsters2Write.Text, out int flyingMonsters2)
                            && (flyingMonsters2 >= 0 && flyingMonsters2 <= 255))
                        {
                            WriteByte(hProc, spawnFlyingMonsters2AddressGlobal, Convert.ToByte(flyingMonsters2));
                        }
                        else
                        {
                            textBoxLog.AppendText("Flying Monsters 2 value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Flying Monsters 2 value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxFlyingMonsters2Read.Text = DisplayByteValue(hProc, spawnFlyingMonsters2AddressGlobal, BASE_ADDRESS_1, OFFSET_SPAWN_FLYING_MONSTERS_2);

                #endregion

                #region Demo Countdown

                if (checkBoxDemoTimer1.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxDemoTimer1Write.Text, out int demoTimer1)
                            && (demoTimer1 >= 0 && demoTimer1 <= 255))
                        {
                            WriteByte(hProc, demoCountdownTimer1AddressGlobal, Convert.ToByte(demoTimer1));
                        }
                        else
                        {
                            textBoxLog.AppendText("Demo Timer 1 value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Demo Timer 1 value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxDemoTimer1Read.Text = DisplayByteValue(hProc, demoCountdownTimer1AddressGlobal, BASE_ADDRESS_1, OFFSET_DEMO_COUNTDOWN_TIMER_1);

                if (checkBoxDemoTimer2.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxDemoTimer2Write.Text, out int demoTimer2)
                            && (demoTimer2 >= 0 && demoTimer2 <= 255))
                        {
                            WriteByte(hProc, demoCountdownTimer1AddressGlobal, Convert.ToByte(demoTimer2));
                        }
                        else
                        {
                            textBoxLog.AppendText("Demo Timer 2 value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("Demo Timer 2 value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxDemoTimer2Read.Text = DisplayByteValue(hProc, demoCountdownTimer2AddressGlobal, BASE_ADDRESS_1, OFFSET_DEMO_COUNTDOWN_TIMER_2);

                #endregion

                #region HP Bar

                if (checkBoxP1HPBar.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP1HPBarWrite.Text, out int p1HpBar)
                            && (p1HpBar >= 0 && p1HpBar <= 255))
                        {
                            WriteByte(hProc, p1HpBarAddressGlobal, Convert.ToByte(p1HpBar));
                        }
                        else
                        {
                            textBoxLog.AppendText("P1 HP Bar value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("P1 HP Bar value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1HPBarRead.Text = DisplayByteValue(hProc, p1HpBarAddressGlobal, BASE_ADDRESS_2, OFFSET_P1_HP_BAR);

                #endregion

                #region Invincibility Timer

                if (checkBoxP1InvincibilityTimer.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP1InvincibilityTimerWrite.Text, out int invincibilityTimer)
                            && (invincibilityTimer >= 0 && invincibilityTimer <= 255))
                        {
                            WriteByte(hProc, p1InvincibilitytimerAddressGlobal, Convert.ToByte(invincibilityTimer));
                        }
                        else
                        {
                            textBoxLog.AppendText("P1 Invincibility Timer value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("P1 Invincibility Timer value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1InvincibilityTimerRead.Text = DisplayByteValue(hProc, p1InvincibilitytimerAddressGlobal, BASE_ADDRESS_2, OFFSET_P1_INVINCIBILITY_TIMER);

                #endregion

                #region POW Bar

                if (checkBoxP1PowBar.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP1PowBarWrite.Text, out int p1PowBar)
                            && (p1PowBar >= 0 && p1PowBar <= 255))
                        {
                            WriteByte(hProc, p1PowBarAddressGlobal, Convert.ToByte(p1PowBar));
                        }
                        else
                        {
                            textBoxLog.AppendText("P1 POW Bar value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("P1 POW Bar value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1PowBarRead.Text = DisplayByteValue(hProc, p1PowBarAddressGlobal, BASE_ADDRESS_2, OFFSET_P1_POW_BAR);

                #endregion

                #region Lives

                if (checkBoxP1Lives.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP1LivesWrite.Text, out int p1Lives)
                            && (p1Lives >= 0 && p1Lives <= 255))
                        {
                            WriteByte(hProc, p1LivesAddressGlobal, Convert.ToByte(p1Lives));
                        }
                        else
                        {
                            textBoxLog.AppendText("P1 Lives value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("P1 Lives value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1LivesRead.Text = DisplayByteValue(hProc, p1LivesAddressGlobal, BASE_ADDRESS_2, OFFSET_P1_LIVES);

                #endregion

                #region Form

                if (checkBoxP1Form.Checked)
                {

                    try
                    {

                        WriteByte(hProc, p1Form1AddressGlobal, Convert.ToByte(((KeyValuePair<int, string>)comboBoxP1FormWrite.SelectedItem).Key));
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred setting Player 1 Form 1."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                if (int.TryParse(DisplayByteValue(hProc, p1Form1AddressGlobal, BASE_ADDRESS_2, OFFSET_P1_FORM_1), out int p1Form1))
                {
                    try
                    {
                        comboBoxP1FormRead.SelectedIndex = p1Form1;
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred getting Player 1 Form 1."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }

                if (checkBoxP1Form2.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP1Form2Write.Text, out int p1Form2)
                            && (p1Form2 >= 0 && p1Form2 <= 255))
                        {
                            WriteByte(hProc, p1Form2AddressGlobal, Convert.ToByte(p1Form2));
                        }
                        else
                        {
                            textBoxLog.AppendText("P1 Form 2 value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("P1 Form 2 value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1Form2Read.Text = DisplayByteValue(hProc, p1Form2AddressGlobal, BASE_ADDRESS_2, OFFSET_P1_FORM_2);

                if (checkBoxP1Form3.Checked)
                {
                    try
                    {
                        if (int.TryParse(textBoxP1Form3Write.Text, out int p1Form3)
                            && (p1Form3 >= 0 && p1Form3 <= 255))
                        {
                            WriteByte(hProc, p1Form3AddressGlobal, Convert.ToByte(p1Form3));
                        }
                        else
                        {
                            textBoxLog.AppendText("P1 Form 3 value must be between 0 and 255." + Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("P1 Form 3 value must be between 0 and 255."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }
                textBoxP1Form3Read.Text = DisplayByteValue(hProc, p1Form3AddressGlobal, BASE_ADDRESS_2, OFFSET_P1_FORM_3);





                if (checkBoxFormLeftOrb1.Checked)
                {
                    try
                    {
                        int writeValue = 0;

                        // Get the value for the orbs
                        if (int.TryParse(DisplayByteValue(hProc, p1Form4AddressGlobal, BASE_ADDRESS_2, OFFSET_P1_FORM_4), out int orbsValueForWrite1))
                        {
                            // Get the value for the current right orb
                            int p1RightOrb2 = GetRightOrb2Value(orbsValueForWrite1);
                            // Get the value for the left orb from the form
                            int p1SelectedLeftOrb = (((KeyValuePair<int, string>)comboBoxFormLeftOrb1Write.SelectedItem).Key);
                            // Add them together
                            writeValue = p1RightOrb2 + p1SelectedLeftOrb;
                        }
                        // Write the combined value of the left and right orbs
                        WriteByte(hProc, p1Form4AddressGlobal, Convert.ToByte(writeValue));
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred setting Player 1 Orb 1."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }

                if (checkBoxFormRightOrb2.Checked)
                {
                    try
                    {
                        int writeValue = 0;

                        // Get the value for the orbs
                        if (int.TryParse(DisplayByteValue(hProc, p1Form4AddressGlobal, BASE_ADDRESS_2, OFFSET_P1_FORM_4), out int orbsValueForWrite2))
                        {
                            // Get the value for the current left orb
                            int p1leftOrb1 = GetLeftOrb1Value(orbsValueForWrite2);
                            // Get the value for the right orb from the form
                            int p1SelectedRightOrb = (((KeyValuePair<int, string>)comboBoxFormRightOrb2Write.SelectedItem).Key);
                            // Add them together
                            writeValue = p1leftOrb1 + p1SelectedRightOrb;
                        }
                        // Write the combined value of the left and right orbs
                        WriteByte(hProc, p1Form4AddressGlobal, Convert.ToByte(writeValue));
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred setting Player 1 Orb 2."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }

                if (int.TryParse(DisplayByteValue(hProc, p1Form4AddressGlobal, BASE_ADDRESS_2, OFFSET_P1_FORM_4), out int orbsValue))
                {
                    int p1LeftOrb1 = GetLeftOrb1Value(orbsValue);
                    int p1RightOrb2 = GetRightOrb2Value(orbsValue);

                    try
                    {
                        if (p1LeftOrb1 == 0)
                        {
                            comboBoxFormLeftOrb1Read.SelectedIndex = 0;
                        }
                        else if (p1LeftOrb1 == 4)
                        {
                            comboBoxFormLeftOrb1Read.SelectedIndex = 1;
                        }
                        else if (p1LeftOrb1 == 8)
                        {
                            comboBoxFormLeftOrb1Read.SelectedIndex = 2;
                        }
                        else if (p1LeftOrb1 == 12)
                        {
                            comboBoxFormLeftOrb1Read.SelectedIndex = 3;
                        }
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred getting Player 1 Orb 1."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }

                    try
                    {
                        comboBoxFormRightOrb2Read.SelectedIndex = p1RightOrb2;
                    }
                    catch (Exception ex)
                    {
                        textBoxLog.AppendText("An error occurred getting Player 1 Orb 2."
                            + Environment.NewLine
                            + "Exception: "
                            + ex);
                    }
                }

                #endregion

            }
        }

        private int GetLeftOrb1Value(int orbsValue)
        {
            int orb1Value = 0;

            bool isMultiple1 = orbsValue % 1 == 0;
            bool isMultiple2 = orbsValue % 2 == 0;
            bool isMultiple3 = orbsValue % 3 == 0;
            bool isMultiple4 = orbsValue % 4 == 0;
            bool isMultiple5 = orbsValue % 5 == 0;
            bool isMultiple6 = orbsValue % 6 == 0;
            bool isMultiple7 = orbsValue % 7 == 0;
            bool isMultiple8 = orbsValue % 8 == 0;
            bool isMultiple9 = orbsValue % 9 == 0;
            bool isMultiple10 = orbsValue % 10 == 0;
            bool isMultiple11 = orbsValue % 11 == 0;
            bool isMultiple12 = orbsValue % 12 == 0;
            bool isMultiple13 = orbsValue % 13 == 0;
            bool isMultiple14 = orbsValue % 14 == 0;
            bool isMultiple15 = orbsValue % 15 == 0;
            bool isMultiple16 = orbsValue % 16 == 0;

            if (orbsValue == 0)
            {
                orb1Value = 0;
            }
            else if (isMultiple16)
            {
                orb1Value = 0;
            }
            else if (isMultiple15)
            {
                orb1Value = 12;
            }
            else if (isMultiple14)
            {
                orb1Value = 12;
            }
            else if (isMultiple13)
            {
                orb1Value = 12;
            }
            else if (isMultiple12)
            {
                orb1Value = 12;
            }
            else if (isMultiple11)
            {
                orb1Value = 8;
            }
            else if (isMultiple10)
            {
                orb1Value = 8;
            }
            else if (isMultiple9)
            {
                orb1Value = 8;
            }
            else if (isMultiple8)
            {
                orb1Value = 8;
            }
            else if (isMultiple7)
            {
                orb1Value = 4;
            }
            else if (isMultiple6)
            {
                orb1Value = 4;
            }
            else if (isMultiple5)
            {
                orb1Value = 4;
            }
            else if (isMultiple4)
            {
                orb1Value = 4;
            }
            else if (isMultiple3)
            {
                orb1Value = 0;
            }
            else if (isMultiple2)
            {
                orb1Value = 0;
            }
            else if (isMultiple1)
            {
                orb1Value = 0;
            }

            return orb1Value;
        }

        private int GetRightOrb2Value(int orbsValue)
        {
            int orb2Value = 0;

            bool isMultiple1 = orbsValue % 1 == 0;
            bool isMultiple2 = orbsValue % 2 == 0;
            bool isMultiple3 = orbsValue % 3 == 0;
            bool isMultiple4 = orbsValue % 4 == 0;
            bool isMultiple5 = orbsValue % 5 == 0;
            bool isMultiple6 = orbsValue % 6 == 0;
            bool isMultiple7 = orbsValue % 7 == 0;
            bool isMultiple8 = orbsValue % 8 == 0;
            bool isMultiple9 = orbsValue % 9 == 0;
            bool isMultiple10 = orbsValue % 10 == 0;
            bool isMultiple11 = orbsValue % 11 == 0;
            bool isMultiple12 = orbsValue % 12 == 0;
            bool isMultiple13 = orbsValue % 13 == 0;
            bool isMultiple14 = orbsValue % 14 == 0;
            bool isMultiple15 = orbsValue % 15 == 0;
            bool isMultiple16 = orbsValue % 16 == 0;

            if (orbsValue == 0)
            {
                orb2Value = 0;
            }
            else if (isMultiple16)
            {
                orb2Value = 0;
            }
            else if (isMultiple15)
            {
                orb2Value = 3;
            }
            else if (isMultiple14)
            {
                orb2Value = 2;
            }
            else if (isMultiple13)
            {
                orb2Value = 1;
            }
            else if (isMultiple12)
            {
                orb2Value = 0;
            }
            else if (isMultiple11)
            {
                orb2Value = 3;
            }
            else if (isMultiple10)
            {
                orb2Value = 2;
            }
            else if (isMultiple9)
            {
                orb2Value = 1;
            }
            else if (isMultiple8)
            {
                orb2Value = 0;
            }
            else if (isMultiple7)
            {
                orb2Value = 3;
            }
            else if (isMultiple6)
            {
                orb2Value = 2;
            }
            else if (isMultiple5)
            {
                orb2Value = 1;
            }
            else if (isMultiple4)
            {
                orb2Value = 0;
            }
            else if (isMultiple3)
            {
                orb2Value = 3;
            }
            else if (isMultiple2)
            {
                orb2Value = 2;
            }
            else if (isMultiple1)
            {
                orb2Value = 1;
            }

            return orb2Value;
        }
    }
}
