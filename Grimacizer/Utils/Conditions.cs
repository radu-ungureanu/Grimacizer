using Grimacizer7.DAL;
using Grimacizer7.DAL.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grimacizer7.Utils
{
    public class Conditions
    {
        private List<LevelVerifier> _conditions;

        public Conditions()
        {
            _conditions = new List<LevelVerifier>
            {
                new LevelVerifier 
                { 
                    Level = 1, 
                    ConditionText = "Half-opened eyes",
                    Verifier = (defaults, computed) => Level1(defaults, computed)
                }, 
                new LevelVerifier 
                {
                    Level = 2, 
                    ConditionText = "Left eye closed",
                    Verifier = (defaults, computed) => Level2(defaults, computed)
                },
                new LevelVerifier 
                { 
                    Level = 3, 
                    ConditionText = "Right eye closed",
                    Verifier = (defaults, computed) => Level3(defaults, computed)
                },
                new LevelVerifier 
                {
                    Level = 4,
                    ConditionText = "Big eyes, mouth closed",
                    Verifier = (defaults, computed) => Level4(defaults, computed)
                },
                new LevelVerifier 
                { 
                    Level = 5, 
                    ConditionText = "Half-opened eyes, kiss", 
                    Verifier = (defaults, computed) => Level5(defaults, computed)
                },
                new LevelVerifier 
                {
                    Level = 6,
                    ConditionText = "Left eye closed, kiss",
                    Verifier = (defaults, computed) => Level6(defaults, computed)
                },
                new LevelVerifier 
                {
                    Level = 7, 
                    ConditionText = "Right eye closed, kiss", 
                    Verifier = (defaults, computed) => Level7(defaults, computed)
                },
                new LevelVerifier 
                { 
                    Level = 8, 
                    ConditionText = "Big eyes, kiss", 
                    Verifier = (defaults, computed) => Level8(defaults, computed)
                },
                //new LevelVerifier 
                //{
                //    Level = 9, 
                //    ConditionText = "Big eyes, mouth open", 
                //    Verifier = (defaults, computed) => Level9(defaults, computed)
                //},
                //new LevelVerifier 
                //{
                //    Level = 10,
                //    ConditionText = "Eyes closed, mouth open",
                //    Verifier = (defaults, computed) => Level10(defaults, computed)
                //},
                //new LevelVerifier 
                //{
                //    Level = 11, 
                //    ConditionText = "Half-opened eyes, mouth kiss-like",
                //    Verifier = (defaults, computed) => Level11(defaults, computed)
                //},
                //new LevelVerifier 
                //{ 
                //    Level = 12, 
                //    ConditionText = "Left eye closed, mouth kiss-like",
                //    Verifier = (defaults, computed) => Level12(defaults, computed)
                //},
                //new LevelVerifier 
                //{
                //    Level = 13, 
                //    ConditionText = "Right eye closed, mouth kiss-like",
                //    Verifier = (defaults, computed) => Level13(defaults, computed)
                //},
                //new LevelVerifier 
                //{
                //    Level = 14, 
                //    ConditionText = "Big eyes, mouth kiss-like", 
                //    Verifier = (defaults, computed) => Level14(defaults, computed)
                //},
                //new LevelVerifier 
                //{ 
                //    Level = 15, 
                //    ConditionText = "Closed eyes, mouth kiss-like",
                //    Verifier = (defaults, computed) => Level15(defaults, computed)
                //},          
            };
        }

        public string GetConditionByLevel(int level)
        {
            return this._conditions.FirstOrDefault(t => t.Level == level).ConditionText;
        }

        public Func<double[], double[], int> GetLevelVerifierFunction(int level)
        {
            return this._conditions.FirstOrDefault(t => t.Level == level).Verifier;
        }

        private FaceCalculationsItem GetDefaultFaceCalculations()
        {
            using (var db = new GrimacizerContext(GrimacizerContext.ConnectionString))
            {
                return db.FaceCalculations.FirstOrDefault();
            }
        }

        private int Level1(double[] arrayDefault, double[] arrayCurrent)
        {
            int r = 6;

            double arieFataDef = arrayDefault[14];
            double arieFataCurr = arrayCurrent[14];

            double arieOchiStangDef = arrayDefault[4];
            double arieOchiDreptDef = arrayDefault[5];

            double procOchiCurrStang = arrayCurrent[4] / arrayCurrent[14];
            double procOchiCurrDrept = arrayCurrent[5] / arrayCurrent[14];

            double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
            double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

            /* Eye areas lower than default */
            if ((arieOchiDreptCurr >= arieOchiDreptDef) || (arieOchiStangCurr >= arieOchiStangDef)) r--;//return false;

            /* Eye sizes lower than default */
            double dif1 = arrayDefault[0] / arrayCurrent[0];
            double dif2 = arrayDefault[1] / arrayCurrent[1];

            if (arrayDefault[6] <= arrayCurrent[6] * dif1 || arrayDefault[7] <= arrayCurrent[7] * dif2) r -= 2;//return false;

            /* Mouth size */
            if (((arrayCurrent[15] * (arieFataDef / arieFataCurr)) / arrayDefault[15]) > 1.3) r -= 3;//return  false;

            // toate conditiile sunt indeplinite
            if (r == 6) return 3;
            // aria la ochi nu e buna
            if (r == 5) return 2;
            // diststana la pleoape nu e una
            if (r == 4) return 1;
            // strambatura nu e corecta
            return 0;
        }

        private int Level2(double[] arrayDef, double[] arrayCurr)
        {
            int r = 8;
            double arieFataDef = arrayDef[14];
            double arieFataCurr = arrayCurr[14];

            double arieOchiStangDef = arrayDef[4];
            double arieOchiDreptDef = arrayDef[5];

            double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
            double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

            double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
            double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

            /* Eye areas left higher than default */
            if ((arieOchiStangCurr >= arieOchiStangDef)) r -= 1;//return false;

            /* Eye size lower than default */
            double dif1 = arrayDef[0] / arrayCurr[0];
            if (arrayDef[6] <= arrayCurr[6] * dif1) r -= 2;//return false;

            /* Eye left is lower then right */
            if (arieOchiStangCurr >= arieOchiDreptCurr || arrayCurr[6] >= arrayCurr[7]) r -= 3;//return false;

            /* Mouth size */
            if (((arrayCurr[15] * (arieFataDef / arieFataCurr)) / arrayDef[15]) > 1.3) r -= 4;//return false;

            if (r == 8) return 3;
            if (r == 7) return 2;
            if (r == 6) return 1;

            return 0;
            //return true;
        }

        private int Level3(double[] arrayDef, double[] arrayCurr)
        {
            int r = 8;
            double arieFataDef = arrayDef[14];
            double arieFataCurr = arrayCurr[14];

            double arieOchiStangDef = arrayDef[4];
            double arieOchiDreptDef = arrayDef[5];

            double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
            double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

            double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
            double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

            /* Eye areas right higher than default */
            if ((arieOchiDreptCurr >= arieOchiDreptDef)) r--; //return false;

            /* Calculate the zoom of the picture */
            double dif2 = arrayDef[1] / arrayCurr[1];

            if (arrayDef[7] <= arrayCurr[7] * dif2) r -= 2; //return false;

            /* Eye right is lower then left */
            if (arieOchiDreptCurr >= arieOchiStangCurr || arrayCurr[7] >= arrayCurr[6]) r -= 3; //return false;

            /* Mouth size */
            if (((arrayCurr[15] * (arieFataDef / arieFataCurr)) / arrayDef[15]) > 1.3) r -= 4;//return false;

            if (r == 8) return 3;
            if (r == 7) return 2;
            if (r == 6) return 1;

            return 0;
        }

        private int Level4(double[] arrayDef, double[] arrayCurr)
        {
            int r = 6;
            double arieFataDef = arrayDef[14];
            double arieFataCurr = arrayCurr[14];

            double arieOchiStangDef = arrayDef[4];
            double arieOchiDreptDef = arrayDef[5];

            double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
            double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

            double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
            double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

            /* Eye areas higher than default */
            if ((arieOchiDreptCurr <= arieOchiDreptDef) || (arieOchiStangCurr <= arieOchiStangDef)) r--; //return false;

            double dif1 = arrayDef[0] / arrayCurr[0];
            double dif2 = arrayDef[1] / arrayCurr[1];

            if (arrayDef[6] >= arrayCurr[6] * dif1 || arrayDef[7] >= arrayCurr[7] * dif2) r -= 2; //return false;

            /* Mouth size */
            if (((arrayCurr[15] * (arieFataDef / arieFataCurr)) / arrayDef[15]) > 1.3) r -= 3; //return false;

            if (r == 6) return 3;
            if (r == 5) return 2;
            if (r == 4) return 1;

            return 0;
        }

        private int Level5(double[] arrayDef, double[] arrayCurr)
        {
            int r = 6;

            double arieFataDef = arrayDef[14];
            double arieFataCurr = arrayCurr[14];

            double arieOchiStangDef = arrayDef[4];
            double arieOchiDreptDef = arrayDef[5];

            double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
            double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

            double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
            double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

            int nrStelute = 0;

            /* Eye areas lower than default */
            if ((arieOchiDreptCurr >= arieOchiDreptDef) || (arieOchiStangCurr >= arieOchiStangDef))
            {
                nrStelute++;
                r--;
            }

            /* Eye sizes lower than default */
            double dif1 = arrayDef[0] / arrayCurr[0];
            double dif2 = arrayDef[1] / arrayCurr[1];

            if (arrayDef[6] <= arrayCurr[6] * dif1 || arrayDef[7] <= arrayCurr[7] * dif2)
            {
                nrStelute++;
                r -= 2;
            }

            /*// Gura pupic varianta 1 -> lungime gura scalata in functie de arie fata
            double difArieTotala = arrayDef[14] / arrayCurr[14];

            // scalam lungimea gurii in functie de departare/apropiere
            double lgGuraScalat = difArieTotala * arrayCurr[11];
            double lgGuraDefault = arrayDef[11];

            // daca gura e mai ingusta de data asta atunci
            if (lgGuraScalat >= lgGuraDefault)
            {
                nrStelute++;
                r -= 3;
            }
            */

            // Gura pupic varianta 2 -> unghiuri gura
            double unghiStangaDef = arrayDef[9];
            double unghiDreaptaDef = arrayDef[10];

            double unghiStangaCurr = arrayCurr[9];
            double unghiDreaptaCurr = arrayCurr[10];

            // lasam chestia cu unghiurile mai permisive (nu trebuie respectate ambele unghiuri)
            if (!(unghiDreaptaCurr > unghiDreaptaDef || unghiStangaCurr > unghiStangaDef))
            {
                nrStelute++;
                r -= 3;
            }


            /*
            // Gura pupic varianta 3 -> lungime gura scalata in functie de lungime barbie
            double difLungimeBarbie = arrayDef[2] / arrayCurr[2];

            // scalam lungimea gurii in functie de departare/apropiere
            double lgGuraScalat = difLungimeBarbie * arrayCurr[11];
            double lgGuraDefault = arrayDef[11];

            // daca gura e mai ingusta de data asta atunci
            if (lgGuraScalat >= lgGuraDefault) {
                nrStelute++;
                r -= 3;
            }
            */
            if (r == 6) return 3;
            if (r == 5) return 2;
            if (r == 4) return 1;

            return 0;
        }

        private int Level6(double[] arrayDef, double[] arrayCurr)
        {
            int r = 8;
            double arieFataDef = arrayDef[14];
            double arieFataCurr = arrayCurr[14];

            double arieOchiStangDef = arrayDef[4];
            double arieOchiDreptDef = arrayDef[5];

            double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
            double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

            double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
            double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

            /* Eye areas left higher than default */
            if ((arieOchiStangCurr >= arieOchiStangDef)) r -= 1;//return false;

            /* Eye size lower than default */
            double dif1 = arrayDef[0] / arrayCurr[0];
            if (arrayDef[6] <= arrayCurr[6] * dif1) r -= 2;//return false;

            /* Eye left is lower then right */
            if (arieOchiStangCurr >= arieOchiDreptCurr || arrayCurr[6] >= arrayCurr[7]) r -= 3;//return false;

            // Gura pupic varianta 2 -> unghiuri gura
            double unghiStangaDef = arrayDef[9];
            double unghiDreaptaDef = arrayDef[10];

            double unghiStangaCurr = arrayCurr[9];
            double unghiDreaptaCurr = arrayCurr[10];

            // lasam chestia cu unghiurile mai permisive (nu trebuie respectate ambele unghiuri)
            if (!(unghiDreaptaCurr > unghiDreaptaDef || unghiStangaCurr > unghiStangaDef))
            {
                r -= 4;
            }

            if (r == 8) return 3;
            if (r == 7) return 2;
            if (r == 6) return 1;

            return 0;
        }

        private int Level7(double[] arrayDef, double[] arrayCurr)
        {
            int r = 8;
            double arieFataDef = arrayDef[14];
            double arieFataCurr = arrayCurr[14];

            double arieOchiStangDef = arrayDef[4];
            double arieOchiDreptDef = arrayDef[5];

            double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
            double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

            double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
            double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

            /* Eye areas right higher than default */
            if ((arieOchiDreptCurr >= arieOchiDreptDef)) r--; //return false;

            /* Calculate the zoom of the picture */
            double dif2 = arrayDef[1] / arrayCurr[1];

            if (arrayDef[7] <= arrayCurr[7] * dif2) r -= 2; //return false;

            /* Eye right is lower then left */
            if (arieOchiDreptCurr >= arieOchiStangCurr || arrayCurr[7] >= arrayCurr[6]) r -= 3; //return false;

            // Gura pupic varianta 2 -> unghiuri gura
            double unghiStangaDef = arrayDef[9];
            double unghiDreaptaDef = arrayDef[10];

            double unghiStangaCurr = arrayCurr[9];
            double unghiDreaptaCurr = arrayCurr[10];

            // lasam chestia cu unghiurile mai permisive (nu trebuie respectate ambele unghiuri)
            if (!(unghiDreaptaCurr > unghiDreaptaDef || unghiStangaCurr > unghiStangaDef))
            {
                r -= 4;
            }

            if (r == 8) return 3;
            if (r == 7) return 2;
            if (r == 6) return 1;

            return 0;
        }

        private int Level8(double[] arrayDef, double[] arrayCurr)
        {
            int r = 6;
            double arieFataDef = arrayDef[14];
            double arieFataCurr = arrayCurr[14];

            double arieOchiStangDef = arrayDef[4];
            double arieOchiDreptDef = arrayDef[5];

            double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
            double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

            double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
            double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

            /* Eye areas higher than default */
            if ((arieOchiDreptCurr <= arieOchiDreptDef) || (arieOchiStangCurr <= arieOchiStangDef)) r--; //return false;

            double dif1 = arrayDef[0] / arrayCurr[0];
            double dif2 = arrayDef[1] / arrayCurr[1];

            if (arrayDef[6] >= arrayCurr[6] * dif1 || arrayDef[7] >= arrayCurr[7] * dif2) r -= 2; //return false;
            // Gura pupic varianta 2 -> unghiuri gura
            double unghiStangaDef = arrayDef[9];
            double unghiDreaptaDef = arrayDef[10];

            double unghiStangaCurr = arrayCurr[9];
            double unghiDreaptaCurr = arrayCurr[10];

            // lasam chestia cu unghiurile mai permisive (nu trebuie respectate ambele unghiuri)
            if (!(unghiDreaptaCurr > unghiDreaptaDef || unghiStangaCurr > unghiStangaDef))
            {
                r -= 3;
            }

            if (r == 6) return 3;
            if (r == 5) return 2;
            if (r == 4) return 1;

            return 0;
        }

        //private int Level9(double[] arrayDef, double[] arrayCurr)
        //{
        //    double arieFataDef = arrayDef[14];
        //    double arieFataCurr = arrayCurr[14];

        //    double arieOchiStangDef = arrayDef[4];
        //    double arieOchiDreptDef = arrayDef[5];

        //    double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
        //    double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

        //    double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
        //    double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

        //    /* Eye areas higher than default */
        //    if ((arieOchiDreptCurr <= arieOchiDreptDef) || (arieOchiStangCurr <= arieOchiStangDef)) return false;

        //    /* Eye sizes higher than default */
        //    double dif1 = arrayDef[0] / arrayCurr[0];
        //    double dif2 = arrayDef[1] / arrayCurr[1];

        //    // Default eyes areas smaller than current eyes areas
        //    if (arrayDef[6] >= arrayCurr[6] * dif1 || arrayDef[7] >= arrayCurr[7] * dif2) return false;

        //    /* Mouth size */
        //    if (((arrayCurr[15] * (arieFataDef / arieFataCurr)) / arrayDef[15]) > 1.3) return false;

        //    return true;
        //}

        //private int Level10(double[] arrayDef, double[] arrayCurr)
        //{
        //    double arieFataDef = arrayDef[14];
        //    double arieFataCurr = arrayCurr[14];

        //    double arieOchiStangDef = arrayDef[4];
        //    double arieOchiDreptDef = arrayDef[5];

        //    double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
        //    double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

        //    double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
        //    double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

        //    // Current eyes areas smaller than default eyes areas
        //    if ((arieOchiDreptCurr >= arieOchiDreptDef) || (arieOchiStangCurr >= arieOchiStangDef)) return false;


        //    // Eyes area differences = 40 percent
        //    double dif_och = arieOchiStangDef - arieOchiStangCurr;
        //    double pr = (100 * dif_och) / arieOchiStangCurr;
        //    if (pr >= 40) return false;


        //    dif_och = arieOchiDreptDef - arieOchiDreptCurr;
        //    pr = (100 * dif_och) / arieOchiDreptCurr;
        //    if (pr >= 40) return false;

        //    // Current left eye length smaller than default left eye length        
        //    if (arrayCurr[6] >= arrayDef[6]) return false;
        //    // Current right eye length smaller than default right eye length
        //    if (arrayCurr[7] >= arrayDef[7]) return false;

        //    // Current eyes areas differences bigger than default eyes areas with 30 percent
        //    dif_och = arrayDef[7] - arrayCurr[7];
        //    pr = (100 * dif_och) / arrayDef[7];
        //    if (pr >= 30) return false;

        //    dif_och = arrayDef[7] - arrayCurr[7];
        //    pr = (100 * dif_och) / arrayDef[7];
        //    if (pr >= 30) return false;


        //    // Current mouth size same as default
        //    double dif_gura = Math.Abs(arrayCurr[8] - arrayDef[8]);
        //    double procent = (100 * dif_gura) / arrayDef[8];
        //    if (procent <= 20) return false;

        //    dif_gura = Math.Abs(arrayCurr[11] - arrayDef[11]);
        //    procent = (100 * dif_gura) / arrayDef[11];
        //    if (procent <= 20) return false;


        //    return true;
        //}

        //private int Level11(double[] arrayDef, double[] arrayCurr)
        //{

        //    double arieFataDef = arrayDef[14];
        //    double arieFataCurr = arrayCurr[14];

        //    double arieOchiStangDef = arrayDef[4];
        //    double arieOchiDreptDef = arrayDef[5];

        //    double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
        //    double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

        //    double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
        //    double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

        //    /* Current eyes areas smaller than default eyes areas */
        //    if ((arieOchiDreptCurr >= arieOchiDreptDef) || (arieOchiStangCurr >= arieOchiStangDef)) return false;

        //    double dif1 = arrayDef[0] / arrayCurr[0];
        //    double dif2 = arrayDef[1] / arrayCurr[1];

        //    /* Current eyes length smaller than default eyes length */
        //    if (arrayDef[6] <= arrayCurr[6] * dif1 || arrayDef[7] <= arrayCurr[7] * dif2) return false;

        //    /* Chin differences (current vs. default) percentage  */
        //    double procBarbie = arrayDef[2] / arrayCurr[2];

        //    /* Mouth size */
        //    if (arrayCurr[11] * procBarbie >= arrayDef[11]) return false;

        //    return true;
        //}

        //private int Level12(double[] arrayDef, double[] arrayCurr)
        //{

        //    double arieFataDef = arrayDef[14];
        //    double arieFataCurr = arrayCurr[14];

        //    double arieOchiStangDef = arrayDef[4];
        //    double arieOchiDreptDef = arrayDef[5];

        //    double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
        //    double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

        //    double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
        //    double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

        //    /* Current left eye area smaller than default left eye area */
        //    if ((arieOchiStangCurr >= arieOchiStangDef)) return false;

        //    /* Current eyes length smaller than default eyes length */
        //    double dif1 = arrayDef[0] / arrayCurr[0];
        //    if (arrayDef[6] <= arrayCurr[6] * dif1) return false;

        //    /* Current left eye area or length smaller than current right eye area or length */
        //    if (arieOchiStangCurr >= arieOchiDreptCurr || arrayCurr[6] >= arrayCurr[7]) return false;

        //    /* Chin differences (current vs. default) percentage */
        //    double procBarbie = arrayDef[2] / arrayCurr[2];

        //    /* Mouth size differences - current smaller than default */
        //    if (arrayCurr[11] * procBarbie >= arrayDef[11]) return false;

        //    return true;
        //}

        //private int Level13(double[] arrayDef, double[] arrayCurr)
        //{
        //    double arieFataDef = arrayDef[14];
        //    double arieFataCurr = arrayCurr[14];

        //    double arieOchiStangDef = arrayDef[4];
        //    double arieOchiDreptDef = arrayDef[5];

        //    double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
        //    double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

        //    double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
        //    double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

        //    /* Current right eye area smaller than default right eye area */
        //    if ((arieOchiDreptCurr >= arieOchiDreptDef)) return false;

        //    /* Eyes length  */
        //    double dif2 = arrayDef[1] / arrayCurr[1];
        //    if (arrayDef[7] <= arrayCurr[7] * dif2) return false;

        //    /* Current right eye length or area smaller than current left eye length or area */
        //    if (arieOchiDreptCurr >= arieOchiStangCurr || arrayCurr[7] >= arrayCurr[6]) return false;

        //    /* Chin differences (current vs. default percentage) */
        //    double procBarbie = arrayDef[2] / arrayCurr[2];

        //    /* current mouth length smaller than default */
        //    if (arrayCurr[11] * procBarbie >= arrayDef[11]) return false;

        //    return true;

        //}

        //private int Level14(double[] arrayDef, double[] arrayCurr)
        //{
        //    double arieFataDef = arrayDef[14];
        //    double arieFataCurr = arrayCurr[14];
        //    double arieOchiStangDef = arrayDef[4];
        //    double arieOchiDreptDef = arrayDef[5];

        //    double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
        //    double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

        //    double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
        //    double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;

        //    /* Current eyes areas bigger than default eyes areas */
        //    if ((arieOchiDreptCurr <= arieOchiDreptDef) || (arieOchiStangCurr <= arieOchiStangDef)) return false;

        //    double dif1 = arrayDef[0] / arrayCurr[0];
        //    double dif2 = arrayDef[1] / arrayCurr[1];

        //    /* current eyes length bigger than default eyes areas */
        //    if (arrayDef[6] >= arrayCurr[6] * dif1 || arrayDef[7] >= arrayCurr[7] * dif2) return false;

        //    /* Chin differences (default vs. current) percentage */
        //    double procBarbie = arrayDef[2] / arrayCurr[2];

        //    /* Current mouth size smaller than default */
        //    if (arrayCurr[11] * procBarbie >= arrayDef[11]) return false;

        //    return true;
        //}

        //private int Level15(double[] arrayDef, double[] arrayCurr)
        //{
        //    double arieFataDef = arrayDef[14];
        //    double arieFataCurr = arrayCurr[14];

        //    double arieOchiStangDef = arrayDef[4];
        //    double arieOchiDreptDef = arrayDef[5];

        //    double procOchiCurrStang = arrayCurr[4] / arrayCurr[14];
        //    double procOchiCurrDrept = arrayCurr[5] / arrayCurr[14];

        //    double arieOchiStangCurr = procOchiCurrStang * arieFataDef;
        //    double arieOchiDreptCurr = procOchiCurrDrept * arieFataDef;


        //    // aria ochiului stang 
        //    // Current left eye area smaller than default left eye area
        //    if (arieOchiStangCurr >= arieOchiStangDef) return false;
        //    // Current right eye area smaller than default right eye area
        //    if (arieOchiDreptCurr >= arieOchiDreptDef) return false;

        //    // Eyes areas differences = 40 percent
        //    double dif_och = arieOchiStangDef - arieOchiStangCurr;
        //    double pr = (100 * dif_och) / arieOchiStangDef;
        //    if (pr >= 40) return false;

        //    dif_och = arieOchiDreptDef - arieOchiDreptCurr;
        //    pr = (100 * dif_och) / arieOchiDreptDef;
        //    if (pr >= 40) return false;

        //    // Current left eye length smaller than default left eye length      
        //    if (arrayCurr[6] >= arrayDef[6]) return false;
        //    // Current right eye length smaller than default right eye length 
        //    if (arrayCurr[7] >= arrayDef[7]) return false;

        //    dif_och = arrayDef[6] - arrayCurr[6];
        //    pr = (100 * dif_och) / arrayDef[6];
        //    if (pr >= 30) return false;

        //    dif_och = arrayDef[7] - arrayCurr[7];
        //    pr = (100 * dif_och) / arrayDef[7];
        //    if (pr >= 30) return false;

        //    // Mouth vertical length bigger than default with 40 percent
        //    if (arrayDef[11] <= arrayCurr[11]) return false;

        //    double dif_gura = arrayDef[11] - arrayCurr[11];
        //    double procent = (100 * dif_gura) / arrayDef[11];
        //    if (procent <= 20) return false;


        //    return true;
        //}
    }

    public class LevelVerifier
    {
        public int Level { get; set; }
        public string ConditionText { get; set; }
        public Func<double[], double[], int> Verifier { get; set; }
    }
}
