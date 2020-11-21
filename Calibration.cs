using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_Forms
{
    public class Calibration
    {
        private float U1_Plus;
        private float U1_Minus;
        private float U2_Plus;
        private float U2_Minus;

        private ushort U1_PlusCode;
        private ushort U1_MinusCode;
        private ushort U2_PlusCode;
        private ushort U2_MinusCode;

        public float Gain_Up;
        public float Gain_Um;

        public float B_Up;
        public float B_Um;
        public float K_Up;
        public float K_Um;


        public Calibration()
        {
            U1_Plus = 0;
            U1_Minus = 0;
            U2_Plus = 0;
            U2_Minus = 0;
            Gain_Up = 0.0f;
            Gain_Um = 0.0f;

            U1_PlusCode = 0;
            U1_MinusCode = 0;
            U2_PlusCode = 0;
            U2_MinusCode = 0;

            B_Up = 0.0f;
            B_Um = 0.0f;
            K_Up = 0.0f;
            K_Um = 0.0f;

        }
        public void Set_UPlus(float U1, float U2)
        {
            U1_Plus = U1;
            U2_Plus = U2;
        }

        public void Set_UMinus(float U1, float U2)
        {
            U1_Minus = U1;
            U2_Minus = U2;
        }

        public void Set_UPlusCode(ushort U1, ushort U2)
        {
            U1_PlusCode = U1;
            U2_PlusCode = U2;
        }

        public void Set_UMinusCode(ushort U1, ushort U2)
        {
            U1_MinusCode = U1;
            U2_MinusCode = U2;
        }

        public void Calc_Koeff_Uplus()
        {
            B_Up = (U2_Plus * U1_PlusCode - U1_Plus * U2_PlusCode) / (U1_PlusCode - U2_PlusCode);
            K_Up = (U1_Plus - B_Up) / U1_PlusCode;
        }

        public void Calc_Koeff_Uminus()
        {
            B_Um = (U2_Minus * U1_MinusCode - U1_Minus * U2_MinusCode) / (U1_MinusCode - U2_MinusCode);
            K_Um = (U1_Minus - B_Um) / U1_MinusCode;
        }
    }
}
