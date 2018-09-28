using System;
using System.Collections.Generic;

namespace AsmVM
{
    using BITS = UInt16;
    public delegate bool Instruction(State x);
    public class State
    {
        public static ulong MEMORY_SIZE       = 65536;
        public static BITS  REGISTER_DEFAULT  = 0;
        public static BITS  INSTRUCTION_START = 0;
        public static BITS  STACK_START       = 65535;

        public byte[] Memory = new byte[MEMORY_SIZE];
        public Dictionary<BITS, BITS> Registers = new Dictionary<BITS, BITS>()
        {
            { 0x0000, INSTRUCTION_START } // IP
          , { 0x0001, STACK_START       } // SP
          , { 0x0002, REGISTER_DEFAULT  } // A
          , { 0x0003, REGISTER_DEFAULT  } // B
          , { 0x0004, REGISTER_DEFAULT  } // C
          , { 0x0005, REGISTER_DEFAULT  } // D
        };
        public BITS REG_IP
        {
            get => Registers[0];
            set => Registers[0] = value;
        }
        public BITS REG_SP
        {
            get => Registers[1];
            set => Registers[1] = value;
        }

        // memory r/w
        public ushort RDMU16(BITS index) =>
            (ushort)(Memory[index] << 8 | Memory[index + 1]);
        public void   WDMU16(BITS index, ushort value)
        {
            Memory[index]     = (byte)(value >> 8   );
            Memory[index + 1] = (byte)(value &  0xFF);
        }
        public byte RDMU81(BITS index) =>
            Memory[index];
        public byte RDMU82(BITS index) =>
            Memory[index + 1];
        public void WDMU81(BITS index, byte value) =>
            Memory[index]     = (byte)(value >> 8);
        public void WDMU82(BITS index, byte value) =>
            Memory[index + 1] = (byte)(value >> 8);
        // register r/w
        public ushort RDRU16(BITS index) =>
            Registers[index];
        public void WDRU16(BITS index, ushort value) =>
            Registers[index] = value;
        public byte RDRU81(BITS index) =>
            (byte)(Registers[index] >> 8   );
        public byte RDRU82(BITS index) =>
            (byte)(Registers[index] &  0xFF);
        public void WDRU81(BITS index, byte value) =>
            Registers[index] = (ushort)(value << 8 | Registers[index] & 0xFF);
        public void WDRU82(BITS index, byte value) =>
            Registers[index] = (ushort)(Registers[index] & 0xFF00 | value & 0xFF);

        // execute
        public void Execute()
        {
            bool execute = true;
            do
            {
                BITS instruction = RDMU16(REG_IP);
                REG_IP += 2;
                execute = Instructions[instruction](this);
            }
            while (execute);
        }

        public Dictionary<BITS, Instruction> Instructions = new Dictionary<BITS, Instruction>()
        {
            { 0x0000, BRK }
          , { 0x0001, MOV16RC }
          , { 0x0002, MOV16RR }
          , { 0x0003, MOV16MC }
          , { 0x0004, MOV16MR }
        };

        public static bool BRK(State x) =>
            false;
        public static bool MOV16RC(State x)
        {
            BITS ip = x.REG_IP, ip2 = (BITS)(ip + 2u);
            x.WDRU16(ip, ip2);
            x.REG_IP += 4;
            return true;
        }
        public static bool MOV16RR(State x)
        {
            BITS ip = x.REG_IP, ip2 = (BITS)(ip + 2u);
            x.WDRU16(ip, x.RDRU16(ip2));
            x.REG_IP += 4;
            return true;
        }
        public static bool MOV16MC(State x)
        {
            BITS ip = x.REG_IP, ip2 = (BITS)(ip + 2u);
            x.WDMU16(ip, ip2);
            x.REG_IP += 4;
            return true;
        }
        public static bool MOV16MR(State x)
        {
            BITS ip = x.REG_IP, ip2 = (BITS)(ip + 2u);
            x.WDMU16(ip, x.RDRU16(ip2));
            x.REG_IP += 4;
            return true;
        }
    }
}
