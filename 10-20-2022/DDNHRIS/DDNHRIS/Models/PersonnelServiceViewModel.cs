using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DDNHRIS.Models
{
    public class PersonnelServiceViewModel
    {

        //PHIC GOVT SHARE : TABLE 2022


        public tRSPRefFundPersonnelService GetCasualFullYearPS(decimal rateMonth, decimal rateDaily, int salaryGrade, DateTime periodFrom, DateTime periodTo, int hazardCode)
        {
            tRSPRefFundPersonnelService ps = new tRSPRefFundPersonnelService();


            int monthCount = Monthcounter(periodFrom, periodTo);

            decimal annual = rateMonth * monthCount;
            decimal PERA = 2000 * monthCount;
            decimal earnedLeave = (rateMonth / 8) * monthCount;

            decimal hazard = 0;
            decimal subsistence = 0;
            decimal laundry = 0;



            //MID-YEAR BONUS
            decimal bonusMidYear = rateMonth;
            if (periodFrom.Year > 2020)
            {
                if (periodFrom > Convert.ToDateTime("January 15, " + periodTo.Year.ToString()))
                {
                    bonusMidYear = 0;
                }
            }
            //YEAR-END BONUS
            decimal bonusYearEnd = rateMonth;
            if (periodFrom.Year > 2020)
            {
                if (periodFrom > Convert.ToDateTime("October 31, " + periodTo.Year.ToString()))
                {
                    bonusYearEnd = 0;
                }
            }


            decimal cashGifth = 5000;
            decimal clothing = 6000;

            decimal lifeAndRetmnt = 0;
            decimal ecc = 100 * monthCount;
            decimal hdmf = 100 * monthCount;
            decimal phic = 0;

            //HEALTH SERVICES
            if (hazardCode == 1)
            {
                hazard = CalQHazardForHealthMonthly(salaryGrade, Convert.ToDecimal(rateDaily)) * monthCount;
                subsistence = 1500 * monthCount;
                laundry = 150 * monthCount;
            }
            else if (hazardCode == 2)  //SOCIAL WORKER
            {
                hazard = CalQHazardForSocialWorkerMonthly(rateDaily) * monthCount;
                subsistence = 1500 * monthCount;
                laundry = 0;
            }

            lifeAndRetmnt = (Convert.ToDecimal(rateMonth) * Convert.ToDecimal(.12)) * monthCount;
            phic = ComputePHICGovtShare(Convert.ToDouble(rateMonth), monthCount);

            decimal total = annual + PERA + earnedLeave + hazard + laundry + subsistence + bonusMidYear + bonusYearEnd + cashGifth + lifeAndRetmnt + ecc + phic + clothing;

            ps.dailyRate = rateDaily;
            ps.monthlyRate = rateMonth;

            ps.RA = 0;
            ps.TA = 0;
            ps.loyalty = 0;

            ps.annualRate = annual;
            ps.PERA = PERA;
            ps.earnedLeave = earnedLeave;
            ps.hazard = hazard;
            ps.laundry = laundry;
            ps.subs = subsistence;
            ps.bonusMY = bonusMidYear;
            ps.bonusYE = bonusYearEnd;
            ps.cashGift = cashGifth;
            ps.clothing = clothing;
            ps.gsis = lifeAndRetmnt;
            ps.ecc = ecc;
            ps.hdmf = hdmf;
            ps.phic = phic;
            ps.total = total;

            ps.monthCount = monthCount;
            ps.periodFrom = periodFrom;
            ps.periodTo = periodTo;
            ps.rateType = "D";
            ps.tag = 1;

            return ps;

        }


        private int Monthcounter(DateTime dFrom, DateTime dTo)
        {
            int res = 0;
            for (int i = 0; i < 12; i++)
            {
                res = res + 1;
                if (dFrom.Month == dTo.Month && dFrom.Year == dTo.Year)
                {
                    return res;
                }
                dFrom = dFrom.AddMonths(1);
            }
            return res;
        }


        private decimal ComputePHICGovtShare(double rateMonth, int monthCount)
        {
            decimal monthlyPrem = 400;

            if (rateMonth >= 10000.01 && rateMonth < 80000)
            {
                monthlyPrem = (Convert.ToDecimal(rateMonth) * Convert.ToDecimal(.04)) / 2;
            }
            else if (rateMonth >= 80000)
            {
                monthlyPrem = 3200 / 2;
            }
            return monthlyPrem * monthCount;
        }

        //HAZARD - SOCIAL WORKER
        private decimal CalQHazardForSocialWorkerMonthly(decimal dailyRate)
        {
            decimal dailyHazard = dailyRate * Convert.ToDecimal(.20);
            decimal hazardPay = (dailyHazard * 30);
            return hazardPay;
        }


        //HAZARD - Health
        private decimal CalQHazardForHealthMonthly(int SG, decimal dailyRate)
        {
            double prcntg = 0;

            if (SG <= 19)
            {
                prcntg = .25;
            }
            else if (SG == 20)
            {
                prcntg = .15;
            }
            else if (SG == 21)
            {
                prcntg = .13;
            }
            else if (SG == 22)
            {
                prcntg = .12;
            }
            else if (SG == 23)
            {
                prcntg = .11;
            }
            else if (SG == 24)
            {
                prcntg = .10;
            }
            else if (SG == 25)
            {
                prcntg = .10;
            }
            else if (SG == 26)
            {
                prcntg = .09;
            }
            else if (SG == 27)
            {
                prcntg = .08;
            }
            else if (SG == 28)
            {
                prcntg = .07;
            }
            else
            {
                prcntg = .05;
            }

            decimal monthlyHazard = 0;
            decimal monthlyRate = Convert.ToDecimal(dailyRate * 22);
            monthlyHazard = (monthlyRate * Convert.ToDecimal(prcntg));
            return monthlyHazard;
        }




    }
}