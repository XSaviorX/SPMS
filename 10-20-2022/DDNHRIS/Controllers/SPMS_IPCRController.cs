using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DDNHRIS.Models.SPMS;

namespace DDNHRIS.Controllers
{
    public class SPMS_IPCRController : Controller
    {
        // GET: SPMS_IPCR
        SPMSDBEntities7 _db = new SPMSDBEntities7();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string nums = "0123456789";
        Random random = new Random();
        string[] quan = new string[5];
        public ActionResult SPMS_IPCR()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ipcr_getList(string officeId, string UserId, string _divId)
        {
            var dpcr = _db.vSPMS_DPCR.Where(a => a.officeId == officeId & a.divisionId == _divId).ToList();
            var cldata = _db.vSPMS_CheckListMFO.Where(a => a.officeId == officeId).ToList();

            var ipcr = _db.tSPMS_IPCR.Where(a => a.i_EIC == UserId).ToList();
            var ipcrwCat = _db.vSPMS_IPCRwCat.Where(a => a.i_EIC == UserId).OrderBy(a => a.categoryId).ToList();

            var stratTotal = _db.vSPMS_IPCRwCat.Where(a => a.i_EIC == UserId & a.description == "Strategic").Sum(a => a.i_Raverage).ToString();
            var coreTotal = _db.vSPMS_IPCRwCat.Where(a => a.i_EIC == UserId & a.description == "Core").Sum(a => a.i_Raverage).ToString();
            var suppTotal = _db.vSPMS_IPCRwCat.Where(a => a.i_EIC == UserId & a.description == "Support").Sum(a => a.i_Raverage).ToString();
            return Json(new { dpcr, cldata, ipcr, ipcrwCat, stratTotal, coreTotal, suppTotal }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SAVE_IPCR(vSPMS_nMFO_ALL IPCRData, int Committed, String UserId, String EIC, String CommittedTgtId)
        {
            string randomLetters = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
            string randomLNumbers = new string(Enumerable.Repeat(nums, 6).Select(s => s[random.Next(s.Length)]).ToArray());
            var target_id = "TGT" + DateTime.Now.ToString("yyMMdd") + randomLetters.ToUpper() + randomLNumbers;

            var update_tRemaining = _db.tSPMS_TargetCountSI.Where(a => a.targetId== IPCRData.targetId).FirstOrDefault();
            var isExist = _db.tSPMS_IPCR.Where(a => a.i_MFOId == IPCRData.MFOId & a.i_indicatorId == IPCRData.indicatorId & a.i_EIC == EIC).FirstOrDefault();
            var standardExist = _db.tSPMS_MFOStandard.Where(a => a.MFOId == IPCRData.MFOId & a.indicatorId == IPCRData.indicatorId & a.targetId == IPCRData.targetId).ToList();

            if (standardExist != null)
            {
                quan[4] = (((double)Committed * 0.3) + (double)Committed).ToString();
                quan[3] = (((double)Committed * 0.15) + (double)Committed).ToString();
                quan[2] = ((double)Committed).ToString();
                quan[1] = (((double)Committed / 2) + 1).ToString();
                quan[0] = ((double)Committed / 2).ToString();


                var newTarget = new tSPMS_TargetCountSI()
                {
                    targetId = target_id,
                    indicatorId = IPCRData.indicatorId,
                    officeId = IPCRData.officeId,
                    target = Committed,
                    tRemaining = Committed
                };
                _db.tSPMS_TargetCountSI.Add(newTarget);

                foreach (var ndata in standardExist)
                {
                    var newStandard = new tSPMS_MFOStandard()
                    {
                        targetId = target_id,
                        MFOId = IPCRData.MFOId,
                        indicatorId = IPCRData.indicatorId,
                        quantity = quan[int.Parse(ndata.rating) - 1].ToString(),
                        quality = ndata.quality,
                        timeliness = ndata.timeliness,
                        rating = ndata.rating
                    };
                    _db.tSPMS_MFOStandard.Add(newStandard);

                }
            }

            if (CommittedTgtId != null)
            {
                var removePrevTgt = _db.tSPMS_TargetCountSI.Where(a => a.targetId == CommittedTgtId).FirstOrDefault();
                _db.tSPMS_TargetCountSI.Remove(removePrevTgt);


                for (int index = 5; index >= 1; index--)
                {
                    var del = _db.tSPMS_MFOStandard.Where(a => a.targetId == CommittedTgtId && a.rating == index.ToString()).FirstOrDefault();
                    _db.tSPMS_MFOStandard.Remove(del);

                }
            } 
            var text = "";
            //var req_id = "REQIPCR"+UserId+IPCRData.year+IPCRData.semester;
            var req_id = "REQIPCR" + UserId + "20222";
            if (isExist == null)
            {
                text = "walay sulod";
                var IPCR = new tSPMS_IPCR()
                {
                    r_ipcrId = req_id,
                    i_EIC = UserId,
                    i_MFOId = IPCRData.MFOId,
                    i_ipcrId = UserId + IPCRData.MFOId,
                    i_actQuantity = 0,
                    i_actQuality = "",
                    i_actTimeliness = "",
                    i_indicatorId = IPCRData.indicatorId,
                    i_target = Committed,
                    i_Rquantity = 0,
                    i_Rquality = 0,
                    i_Rtimeliness = 0,
                    i_Raverage = 0,
                    targetId = target_id,
                    i_category = IPCRData.categoryId,
                    targetIdparent = IPCRData.targetId,
                };
                _db.tSPMS_IPCR.Add(IPCR);
                update_tRemaining.tRemaining = IPCRData.tRemaining;
            }
            else
            {
                update_tRemaining.tRemaining = IPCRData.tRemaining;

                isExist.i_target = Committed;
                isExist.targetId = target_id;
                text = "naay sulod";
            }


            var obj = text;

            _db.SaveChanges();



            /*var updateRemaining = 0;*/
            /*if (isExist == null)
            {
                var IPCR = new tSPMS_IPCR()
                {
                    i_EIC = UserId,
                    i_MFOId = IPCRData.MFOId,
                    i_indicatorId = IPCRData.indicatorId,
                    i_target = Committed
                };
                _db.tSPMS_IPCR.Add(IPCR);
                update_tRemaining.tRemaining = update_tRemaining.tRemaining - Committed;
            }
            else
            {
                // isExist.i_target = Committed;
                var res = 0;

                if (Committed > isExist.i_target)
                {
                    res = Committed - (int)isExist.i_target;
                    updateRemaining = (int)IPCRData.tRemaining - res;
                }
                else
                {
                    res = (int)isExist.i_target - Committed;
                    updateRemaining = (int)IPCRData.tRemaining + res;
                }
                update_tRemaining.tRemaining = IPCRData.tRemaining;

                isExist.i_target = Committed;

            }*/


            return Json(new { data = 1, obj }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult REMOVE_IPCR(vSPMS_nMFO_ALL IPCRData, String UserId, String EIC, String CommittedTgtId)
        {
            var remove = _db.tSPMS_IPCR.Where(a => a.i_MFOId == IPCRData.MFOId & a.i_indicatorId == IPCRData.indicatorId & a.i_EIC == EIC).FirstOrDefault();
           
            var checkRemoved = _db.tSPMS_DPCR.Where(a => a.indicatorId == IPCRData.indicatorId).FirstOrDefault();
            if (checkRemoved != null)
            {
                var update = _db.tSPMS_TargetCountSI.Where(a => a.indicatorId == IPCRData.indicatorId & a.targetId == IPCRData.targetId & a.officeId == IPCRData.officeId).FirstOrDefault();
                update.tRemaining = (update.tRemaining + remove.i_target);
            }

            _db.tSPMS_IPCR.Remove(remove);
            var removeTgt = _db.tSPMS_TargetCountSI.Where(a => a.targetId == CommittedTgtId).FirstOrDefault();
            _db.tSPMS_TargetCountSI.Remove(removeTgt);


            for (int index = 5; index >= 1; index--)
            {
                var del = _db.tSPMS_MFOStandard.Where(a => a.targetId == CommittedTgtId && a.rating == index.ToString()).FirstOrDefault();
                if (del != null)
                {
                    _db.tSPMS_MFOStandard.Remove(del);
                }
            }
            _db.SaveChanges();

            return Json(new { status = 1, remove }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult REMOVE_IPCRMAIN(vSPMS_nMFO_ALL IPCRData, String targetIdparent)
        {
            var remove = _db.tSPMS_IPCR.Where(a => a.targetId == IPCRData.targetId).FirstOrDefault();

            var update = _db.tSPMS_TargetCountSI.Where(a => a.targetId == targetIdparent).FirstOrDefault();

            update.tRemaining = (update.tRemaining + remove.i_target);

            var removeTgt = _db.tSPMS_TargetCountSI.Where(a => a.targetId == IPCRData.targetId).FirstOrDefault();

            _db.tSPMS_TargetCountSI.Remove(removeTgt);
            _db.tSPMS_IPCR.Remove(remove);


            for (int index = 5; index >= 1; index--)
            {
                var del = _db.tSPMS_MFOStandard.Where(a => a.targetId == IPCRData.targetId && a.rating == index.ToString()).FirstOrDefault();
                _db.tSPMS_MFOStandard.Remove(del);

            }

            _db.SaveChanges();

            return Json(new { status = 1, remove }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GET_OTSStandardData(string indicatorId)
        {
            var data = _db.tSPMS_MFOStandard.Where(a => a.indicatorId == indicatorId).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult GET_OTSSelectedMFO(string indicatorId)
        //{
        //  //  var remove = _db.tSPMS_IPCR.Where(a => a.i_MFOId == IPCRData.MFOId & a.i_indicatorId == IPCRData.indicatorId & a.i_EIC == EIC).FirstOrDefault();

        //   // return Json(new { status = 1, remove }, JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public ActionResult SHOW_USERS(vSPMS_nMFO_ALL IPCRDATA)
        {
            var users = _db.vSPMS_IPCRwCat.Where(a => a.i_MFOId == IPCRDATA.MFOId & a.i_indicatorId == IPCRDATA.indicatorId).ToList();
            return Json(new { status = 1, users });
        }

        [HttpPost]
        public ActionResult ADD_OTS(tSPMS_OTS OTS, vSPMS_IPCRwCat MFOData, DateTime dateCreated)
        {
            var month = dateCreated.Month;
            var day = dateCreated.Day;
            var year = dateCreated.Year;
            var _ipcrID = MFOData.i_EIC + MFOData.i_MFOId;

            var addots = new tSPMS_OTS()
            {
                EIC = MFOData.i_EIC,
                ipcrID = MFOData.i_ipcrId,
                MFOId = MFOData.i_MFOId,
                indicatorId = MFOData.i_indicatorId,
                officeId = MFOData.officeId,
                divisionId = MFOData.divisionId,
                //output = OTS.output,
                r_quantity = OTS.r_quantity,
                r_quality = OTS.r_quality,
                r_timeliness = OTS.r_timeliness,
                dateCreated = dateCreated,
                //approvedby = OTS.approvedby,
                month = month,
                day = day,
                status = 0
            };
            //System.Windows.Forms.MessageBox.Show(addots.EIC + "\n" + addots.ipcrID + "\n" + addots.MFOId + "\n" + addots.indicatorId + "\n" + addots.officeId + "\n" + addots.divisionId + "\n");
            _db.tSPMS_OTS.Add(addots);

            _db.SaveChanges();
            var updateQtyVal = _db.tSPMS_IPCR.Where(a => a.targetId == MFOData.targetId).FirstOrDefault();

            int tempqty = (int)updateQtyVal.i_actQuantity;
            int updateTotal = tempqty + (int)OTS.r_quantity;

            updateQtyVal.i_actQuantity = updateTotal;

            int _qtywk1 = 0, _qtywk2 = 0, _qtywk3 = 0, _qtywk4 = 0,
                    _qlywk1 = 0, _qlywk2 = 0, _qlywk3 = 0, _qlywk4 = 0,
                    _timewk1 = 0, _timewk2 = 0, _timewk3 = 0, _timewk4 = 0;

            double _qlyAve = 0, _timeAve = 0;

            if (day >= 1 && day <= 7)
            {
                _qtywk1 = (int)OTS.r_quantity;
                _qlywk1 = (int)OTS.r_quantity * (int)OTS.r_quality;
                _timewk1 = (int)OTS.r_quantity * (int)OTS.r_timeliness;
            }
            else if (day >= 8 && day <= 15)
            {
                _qtywk2 = (int)OTS.r_quantity;
                _qlywk2 = (int)OTS.r_quantity * (int)OTS.r_quality;
                _timewk2 = (int)OTS.r_quantity * (int)OTS.r_timeliness;
            }
            else if (day >= 16 && day <= 22)
            {
                _qtywk3 = (int)OTS.r_quantity;
                _qlywk3 = (int)OTS.r_quantity * (int)OTS.r_quality;
                _timewk3 = (int)OTS.r_quantity * (int)OTS.r_timeliness;
            }
            else if (day >= 23 && day <= 31)
            {
                _qtywk4 = (int)OTS.r_quantity;
                _qlywk4 = (int)OTS.r_quantity * (int)OTS.r_quality;
                _timewk4 = (int)OTS.r_quantity * (int)OTS.r_timeliness;
            }

            var checkMPORifExist = _db.tSPMS_IPCR_MPOR.Where(a => a.ipcrId == MFOData.i_ipcrId & a.month == month).FirstOrDefault();
            if (checkMPORifExist != null)
            {
               // System.Windows.Forms.MessageBox.Show(checkMPORifExist);
                //    var addnewMPORData = new tSPMS_IPCR_MPOR()
                //    {
                //        ipcrId = _ipcrID,
                //        month = month,
                //        year = year,
                //        qtyWk1 = _qtywk1,
                //        qtyWk2 = _qtywk2,
                //        qtyWk3 = _qtywk3,
                //        qtyWk4 = _qtywk4,
                //        qtyTotal = (int)OTS.r_quantity,

                //        qlyWk1 = _qlywk1,
                //        qlyWk2 = _qlywk2,
                //        qlyWk3 = _qlywk3,
                //        qlyWk4 = _qlywk4,
                //        qlyTotal = (int)OTS.r_quantity * (int)OTS.r_quality,

                //        timeWk1 = _timewk1,
                //        timeWk2 = _timewk2,
                //        timeWk3 = _timewk3,
                //        timeWk4 = _timewk4,
                //        timeTotal = (int)OTS.r_quantity * (int)OTS.r_timeliness
                //    };

                //    _db.tSPMS_IPCR_MPOR.Add(addnewMPORData);
                //}
                //else
                //{
                checkMPORifExist.qtyWk1 = checkMPORifExist.qtyWk1 + _qtywk1;
                checkMPORifExist.qtyWk2 = checkMPORifExist.qtyWk2 + _qtywk2;
                checkMPORifExist.qtyWk3 = checkMPORifExist.qtyWk3 + _qtywk3;
                checkMPORifExist.qtyWk4 = checkMPORifExist.qtyWk4 + _qtywk4;
                checkMPORifExist.qtyTotal = checkMPORifExist.qtyTotal + (int)OTS.r_quantity;

                checkMPORifExist.qlyWk1 = checkMPORifExist.qlyWk1 + _qlywk1;
                checkMPORifExist.qlyWk2 = checkMPORifExist.qlyWk2 + _qlywk2;
                checkMPORifExist.qlyWk3 = checkMPORifExist.qlyWk3 + _qlywk3;
                checkMPORifExist.qlyWk4 = checkMPORifExist.qlyWk4 + _qlywk4;
                checkMPORifExist.qlyTotal = checkMPORifExist.qlyTotal + ((int)OTS.r_quality * (int)OTS.r_quantity);

                checkMPORifExist.timeWk1 = checkMPORifExist.timeWk1 + _timewk1;
                checkMPORifExist.timeWk2 = checkMPORifExist.timeWk2 + _timewk2;
                checkMPORifExist.timeWk3 = checkMPORifExist.timeWk3 + _timewk3;
                checkMPORifExist.timeWk4 = checkMPORifExist.timeWk4 + _timewk4;
                checkMPORifExist.timeTotal = checkMPORifExist.timeTotal + ((int)OTS.r_timeliness * (int)OTS.r_quantity);

            }
            _db.SaveChanges();
            var checkMPORifExist2 = _db.tSPMS_IPCR_MPOR.Where(a => a.ipcrId == MFOData.i_ipcrId & a.month == month).FirstOrDefault();
            if (checkMPORifExist2 != null) { 
                var smporCheckExist = _db.tSPMS_IPCR_SMPOR.Where(a => a.ipcrId == MFOData.i_ipcrId).FirstOrDefault();
                if (smporCheckExist != null)
                {
                    if (month == 1 || month == 7)
                    {
                        smporCheckExist.qty_mFirst = checkMPORifExist2.qtyTotal;
                        smporCheckExist.qly_mFirst = checkMPORifExist2.qlyTotal;
                        smporCheckExist.time_mFirst = checkMPORifExist2.timeTotal;
                    }
                    else if (month == 2 || month == 8)
                    {
                        smporCheckExist.qty_mSecond = checkMPORifExist2.qtyTotal;
                        smporCheckExist.qly_mSecond = checkMPORifExist2.qlyTotal;
                        smporCheckExist.time_mSecond = checkMPORifExist2.timeTotal;
                    }
                    else if (month == 3 || month == 9)
                    {
                        smporCheckExist.qty_mThird = checkMPORifExist2.qtyTotal;
                        smporCheckExist.qly_mThird = checkMPORifExist2.qlyTotal;
                        smporCheckExist.time_mThird = checkMPORifExist2.timeTotal;
                    }
                    else if (month == 4 || month == 10)
                    {
                        smporCheckExist.qty_mFourth = checkMPORifExist2.qtyTotal;
                        smporCheckExist.qly_mFourth = checkMPORifExist2.qlyTotal;
                        smporCheckExist.time_mFourth = checkMPORifExist2.timeTotal;
                    }
                    else if (month == 5 || month == 11)
                    {
                        smporCheckExist.qty_mFifth = checkMPORifExist2.qtyTotal;
                        smporCheckExist.qly_mFifth = checkMPORifExist2.qlyTotal;
                        smporCheckExist.time_mFifth = checkMPORifExist2.timeTotal;
                    }
                    else if (month == 6 || month == 12)
                    {
                        smporCheckExist.qty_mSixth = checkMPORifExist2.qtyTotal;
                        smporCheckExist.qly_mSixth = checkMPORifExist2.qlyTotal;
                        smporCheckExist.time_mSixth = checkMPORifExist2.timeTotal;
                    }

                    smporCheckExist.qty_total = smporCheckExist.qty_total + (int)OTS.r_quantity;
                    smporCheckExist.qly_total = smporCheckExist.qly_total + ((int)OTS.r_quality * (int)OTS.r_quantity);
                    smporCheckExist.time_total = smporCheckExist.time_total + ((int)OTS.r_timeliness * (int)OTS.r_quantity);
                    _db.SaveChanges();

                    //update ratings
                    var tempRate = 0.0;
                    for (var cntRate = 5; cntRate > 0; cntRate--) { 
                        var getRate = _db.tSPMS_MFOStandard.Where(a=> a.targetId == MFOData.targetId & a.rating == cntRate.ToString()).FirstOrDefault();
                        if (getRate !=null)
                        {
                            if (getRate.quantity != null)
                            {
                                if (double.Parse(getRate.quantity) >= smporCheckExist.qty_total)
                                {
                                    tempRate = double.Parse(getRate.rating);
                                }
                            }
                        }
                    }
                    var standards = _db.tSPMS_MFOStandard.Where(a=> a.targetId == MFOData.targetId).OrderByDescending(a=> a.rating).ToList();
                    var tempqlyRating = Math.Round((double)(smporCheckExist.qly_total / smporCheckExist.qty_total), 2);
                    var tempTimeRating = Math.Round((double)(smporCheckExist.time_total / smporCheckExist.qty_total), 2);
                    var total = tempRate + tempqlyRating + tempTimeRating;
                    var tempAverage = 0.0;
                    var qlyString = "";
                    var qlyLock = 0;
                    var timeString="";
                    var timeLock = 0;
                    smporCheckExist.qty_rating = tempRate;
                    smporCheckExist.qly_rating = tempqlyRating;
                    smporCheckExist.time_rating = tempTimeRating;

                    if (standards != null)
                    {
                        foreach (var i in standards)
                        {
                            if (tempqlyRating >= double.Parse(i.rating) & qlyLock == 0)
                            {
                                //quality string
                                if ((i.quality != null & i.quality != "") )
                                {
                                    qlyString = i.quality;
                                    qlyLock = 1;
                                }
                            }
                            //timeliness string
                            if (tempTimeRating >= double.Parse(i.rating) & timeLock == 0)
                            {
                                if ((i.timeliness != null & i.timeliness != "") )
                                {
                                    timeString = i.timeliness;
                                    timeLock = 1;
                                }
                            }
                        }
                    }
                    

                    var updateRatings = _db.tSPMS_IPCR.Where(a=> a.i_ipcrId == MFOData.i_ipcrId).FirstOrDefault();
                    if (updateRatings != null)
                    {
                        updateRatings.i_Rquantity = tempRate;
                        updateRatings.i_Rquality = tempqlyRating;
                        updateRatings.i_Rtimeliness = tempTimeRating;
                        updateRatings.i_actQuality = qlyString;
                        updateRatings.i_actTimeliness = timeString;

                        if (total > 0)
                        {
                            tempAverage = total / 3;
                        }
                        updateRatings.i_Raverage = Math.Round(tempAverage,2);

                        

                    }



                }
            }

            _db.SaveChanges();

            return Json(new { status = 1 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SHOW_OTS(tSPMS_IPCR OTS)
        {
            DateTime today = DateTime.Today;
            var curMonth = today.Month;
            var mfoData = _db.vSPMS_IPCRwCat.Where(a => a.i_EIC == OTS.i_EIC & a.i_indicatorId == OTS.i_indicatorId).FirstOrDefault();
            var ots = _db.vSPMS_showOTS.Where(a => a.EIC == OTS.i_EIC & a.indicatorId == OTS.i_indicatorId).ToList();

            return Json(new { status = 1, ots, curMonth, mfoData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult get_StandardQTYEquiv(tSPMS_MFOStandard MFOdata, int qtyVal)
        {
            int equivalent = 0;
            string returnVal = "";
            for (int rating = 5; rating >= 1; rating--)
            {
                var res = _db.tSPMS_MFOStandard.Where(a => a.targetId == MFOdata.targetId & a.MFOId == MFOdata.MFOId & a.indicatorId == MFOdata.indicatorId & a.rating == rating.ToString()).FirstOrDefault();
                if (res != null)
                {
                    if (int.Parse(res.quantity) >= qtyVal)
                    {
                        equivalent = rating;
                    }
                    else
                    {
                        equivalent--;
                        break;
                    }
                }
            }
            returnVal = equivalent.ToString();
            return Json(returnVal, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult getStandard(string targetId)
        {
            var returnStandard = _db.tSPMS_MFOStandard.Where(a => a.targetId == targetId);
            return Json(returnStandard, JsonRequestBehavior.AllowGet);
        }

        //=================== MPOR =======================
        public ActionResult SPMS_IPCR_MPOR()
        {
            return View();
        }
        [HttpPost]
        public JsonResult cookiestarget(string officeId, string EIC)
        {
            Console.Write(EIC);
            HttpCookie cookiname = new HttpCookie("ipcrCrookie")
            {
                Value = EIC + "," + officeId + ","

            };
            Response.Cookies.Add(cookiname);

            return Json(EIC);
        }

        [HttpPost]
        public JsonResult stndCookies(string officeId, string EIC)
        {
            HttpCookie cookiname = new HttpCookie("standardCrookie")
            {
                Value = EIC + "," + officeId + ","

            };
            Response.Cookies.Add(cookiname);

            return Json(EIC);
        }

        [HttpPost]
        public JsonResult actualCookies(string officeId, string EIC)
        {
            HttpCookie cookiname = new HttpCookie("actualCrookie")
            {
                Value = EIC + "," + officeId + ","

            };
            Response.Cookies.Add(cookiname);

            return Json(EIC);
        }

        [HttpPost]
        public JsonResult mporCookies(string officeId, string EIC, int monthId, string month,int year, int semester)
        {
            HttpCookie cookiname = new HttpCookie("mporCrookie")
            {
                Value = EIC + "," + officeId + "," + month + "," + monthId + "," + year + "," + semester + "," 

            };
            Response.Cookies.Add(cookiname);

            return Json(EIC);
        }

        [HttpPost]
        public ActionResult mpor_getData(string EIC, string officeId, int _curMonth)
        {
            var getMPORData = _db.vSPMS_MPOR.Where(a => a.i_EIC == EIC && a.month == _curMonth).ToList();
            return Json(getMPORData, JsonRequestBehavior.AllowGet);
        }


        // ------------  IPCR SUBMITTED ----------------
        public ActionResult SPMS_IPCR_SUBMITTED()
        {

            var pending = (from reqdata in _db.tSPMS_IPCRRequest
                           where reqdata.r_Status == 1
                           select reqdata.r_ipcrId).Distinct().ToList();
            ViewBag.pending = pending.Count();

            var approved = (from reqdata in _db.tSPMS_IPCRRequest
                            where reqdata.r_Status == 2
                            select reqdata.r_ipcrId).Distinct().ToList();
            ViewBag.approved = approved.Count();

            var cancelled = (from reqdata in _db.tSPMS_IPCRRequest
                             where reqdata.r_Status == 3
                             select reqdata.r_ipcrId).Distinct().ToList();
            ViewBag.cancelled = cancelled.Count();

            var totalrequest = (from reqdata in _db.tSPMS_IPCRRequest
                                select reqdata.r_ipcrId).Distinct().ToList();
            ViewBag.totalrequest = totalrequest.Count();

            return View();
        }

        [HttpPost]
        public ActionResult ipcr_submitRequest(string _reqId, string userId)
        {
            var returnVal = 0;
            var checkExist = _db.tSPMS_IPCRRequest.Where(a => a.r_ipcrId == _reqId).FirstOrDefault();

            if (checkExist ==null)
            {
                var newRequest = new tSPMS_IPCRRequest()
                {
                    r_ipcrId = _reqId,
                    r_EIC = userId,
                    r_Status = 0,
                };

                _db.tSPMS_IPCRRequest.Add(newRequest);
                _db.SaveChanges();

                returnVal = 1;
            }


            return Json(returnVal, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ipcr_getListRequest()
        {
            var getRequest = _db.vSPMS_IPCR_Request.ToList();
            return Json(getRequest, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult req_ipcr_getList(String _ipcrId)
        {
            var getList = _db.vSPMS_IPCRwCat.Where(a=> a.r_ipcrId == _ipcrId).OrderBy(a=> a.MFO).ToList();
            return Json(getList, JsonRequestBehavior.AllowGet);
        }

        //-------------------- recommended --------------------
        public ActionResult SPMS_IPCR_RECOMMENDED()
        {

            var pending = (from reqdata in _db.tSPMS_IPCRRequest
                           where reqdata.r_Status == 1
                           select reqdata.r_ipcrId).Distinct().ToList();
            ViewBag.pending = pending.Count();

            var approved = (from reqdata in _db.tSPMS_IPCRRequest
                            where reqdata.r_Status == 2
                            select reqdata.r_ipcrId).Distinct().ToList();
            ViewBag.approved = approved.Count();

            var cancelled = (from reqdata in _db.tSPMS_IPCRRequest
                             where reqdata.r_Status == 3
                             select reqdata.r_ipcrId).Distinct().ToList();
            ViewBag.cancelled = cancelled.Count();

            var totalrequest = (from reqdata in _db.tSPMS_IPCRRequest
                                select reqdata.r_ipcrId).Distinct().ToList();
            ViewBag.totalrequest = totalrequest.Count();

            return View();
        }

        public ActionResult updateRequestStatus(tSPMS_IPCRRequest data, int status)
        {
            var result = 0;
            var ipcrwCat = _db.tSPMS_IPCRRequest.Where(a => a.r_ipcrId == data.r_ipcrId).FirstOrDefault();
            if (ipcrwCat !=null)
            {
                ipcrwCat.r_Status = status;
                result = status;
            }
            _db.SaveChanges();


            // ------ MPOR ADDING OF DATA -------
            if (status == 2)
            {
                var listofIPCR = _db.vSPMS_IPCRwCat.Where(a => a.r_ipcrId == data.r_ipcrId).ToList();

                if (listofIPCR != null)
                {
                    foreach (var items in listofIPCR)
                    {
                        if (items.r_semester == 1)
                        {
                            for (var i = 1; i < 7; i++)
                            {
                                var mporAddNewItem = new tSPMS_IPCR_MPOR()
                                {
                                    ipcrId = items.i_ipcrId,
                                    month = i,
                                    year = items.r_year,
                                    qtyWk1 = 0,
                                    qtyWk2 = 0,
                                    qtyWk3 = 0,
                                    qtyWk4 = 0,
                                    qtyTotal = 0,
                                    qlyWk1 = 0,
                                    qlyWk2 = 0,
                                    qlyWk3 = 0,
                                    qlyWk4 = 0,
                                    qlyTotal = 0,
                                    qlyAve = 0,
                                    timeWk1 = 0,
                                    timeWk2 = 0,
                                    timeWk3 = 0,
                                    timeWk4 = 0,
                                    timeTotal = 0,
                                    timeAve = 0
                                };
                                _db.tSPMS_IPCR_MPOR.Add(mporAddNewItem);
                            }
                            _db.SaveChanges();
                        }
                        else if (items.r_semester == 2)
                        {
                            for (var i = 7; i < 13; i++)
                            {
                                var mporAddNewItem = new tSPMS_IPCR_MPOR()
                                {
                                    ipcrId = items.i_ipcrId,
                                    month = i,
                                    year = items.r_year,
                                    qtyWk1 = 0,
                                    qtyWk2 = 0,
                                    qtyWk3 = 0,
                                    qtyWk4 = 0,
                                    qtyTotal = 0,
                                    qlyWk1 = 0,
                                    qlyWk2 = 0,
                                    qlyWk3 = 0,
                                    qlyWk4 = 0,
                                    qlyTotal = 0,
                                    qlyAve = 0,
                                    timeWk1 = 0,
                                    timeWk2 = 0,
                                    timeWk3 = 0,
                                    timeWk4 = 0,
                                    timeTotal = 0,
                                    timeAve = 0
                                };
                                _db.tSPMS_IPCR_MPOR.Add(mporAddNewItem);
                            }
                            _db.SaveChanges();
                        }

                        // ------ SMPOR ADDING OF DATA -------
                        var smporId = "SMPOR" + items.r_EIC + items.r_year + items.r_semester;

                        var smporAddNewItem = new tSPMS_IPCR_SMPOR()
                        {
                            smporId = smporId,
                            ipcrId = items.i_ipcrId,
                            qty_mFirst = 0,
                            qty_mSecond = 0,
                            qty_mThird = 0,
                            qty_mFourth = 0,
                            qty_mFifth = 0,
                            qty_mSixth = 0,
                            qty_total = 0,
                            qty_rating = 0,
                            qly_mFirst = 0,
                            qly_mSecond = 0,
                            qly_mThird = 0,
                            qly_mFourth = 0,
                            qly_mFifth = 0,
                            qly_mSixth = 0,
                            qly_total = 0,
                            qly_rating = 0,
                            time_mFirst = 0,
                            time_mSecond = 0,
                            time_mThird = 0,
                            time_mFourth = 0,
                            time_mFifth = 0,
                            time_mSixth = 0,
                            time_total = 0,
                            time_rating = 0
                        };
                        _db.tSPMS_IPCR_SMPOR.Add(smporAddNewItem);
                        _db.SaveChanges();
                    }
                }
            }

            



            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // ------------------- IPCR ACTUAL ------------------------

        public ActionResult SPMS_IPCR_ACTUAL()
        {
            return View();
        }


        // -------------------- SMPOR -------------------------------
        public ActionResult SPMS_IPCR_SMPOR()
        {
            return View();
        }

        [HttpPost]
        public ActionResult smpor_getData(string EIC, int _year, int _sem)
        {
            var getSMPORData = _db.vSPMS_IPCR_SMPOR.Where(a => a.i_EIC == EIC & a.r_year == _year & a.r_semester == _sem).ToList();
            return Json(getSMPORData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MFO_getIndicatorDetails(string indId, string targtId)
        {
            var indDetails = _db.vSPMS_IPCRwCat.Where(a => a.i_indicatorId == indId).FirstOrDefault();
            var standardDatas = _db.tSPMS_MFOStandard.Where(a => a.targetId == targtId).ToList();
            return Json(new { indDetails, standardDatas, indId }, JsonRequestBehavior.AllowGet);
        }


    }
}