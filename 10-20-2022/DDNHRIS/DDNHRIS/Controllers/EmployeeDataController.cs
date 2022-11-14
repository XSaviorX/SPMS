using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize(Roles = "EMP")]

    public class EmployeeDataController : Controller
    {
        HRISDBEntities db = new HRISDBEntities();
        // GET: EmployeeData
        public ActionResult PDS()
        {
            //CHECK PDS PROFILE DATA
            string uEIC = Session["_EIC"].ToString();
            tPDSProfile pds = db.tPDSProfiles.SingleOrDefault(e => e.EIC == uEIC);
            if (pds == null)
            {
                tPDSProfile p = new tPDSProfile();
                p.EIC = uEIC;
                db.tPDSProfiles.Add(p);
                db.SaveChanges();
            }



            return View();
        }

        [HttpPost]
        public JsonResult PersonalData()
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                vRSPEmployeeList emp = db.vRSPEmployeeLists.Single(e => e.EIC == uEIC);
                tPDSProfile p = db.tPDSProfiles.SingleOrDefault(e => e.EIC == uEIC);

                TempPDSProfile profile = new TempPDSProfile();

                profile.EIC = emp.EIC;
                profile.lastName = emp.lastName;
                profile.firstName = emp.firstName;
                profile.middleName = emp.middleName;
                profile.extName = emp.extName;

                profile.birthDate = emp.birthDate;
                profile.birthPlace = emp.birthPlace;
                profile.sex = emp.sex;

                profile.civilStatCode = emp.civilStatCode;
                profile.height = emp.height;
                profile.weight = emp.weight;
                profile.bloodType = emp.bloodType;
                profile.GSISIDNo = emp.GSISIDNo;
                profile.PHICNo = emp.PHICNo;
                profile.HDMFNo = emp.PHICNo;
                profile.SSSNo = emp.SSSNo;
                profile.TINNo = emp.TINNo;
                profile.idNo = emp.idNo;
                profile.telephoneNo = emp.telephoneNo;
                profile.mobileNo = emp.mobileNo;
                profile.emailAddress = emp.emailAddress;

                profile.citizenship = Convert.ToInt16(p.citizenship);
                profile.citizenshipType = Convert.ToInt16(p.citizenshipType);
                profile.citizenshipCountry = p.citizenshipCountry;

                profile.addrsResVillage = p.addrsResVillage;
                profile.addrsResHouseBlk = p.addrsResHouseBlk;
                profile.addrsResStreet = p.addrsResStreet;
                profile.addrsResBarangay = p.addrsResBarangay;
                profile.addrsResCityMun = p.addrsResCityMun;
                profile.addrsResProvince = p.addrsResProvince;
                profile.addrsResZip = p.addrsResZip;

                profile.addrsPermVillage = p.addrsPermVillage;
                profile.addrsPermHouseBlk = p.addrsPermHouseBlk;
                profile.addrsPermStreet = p.addrsPermStreet;
                profile.addrsPermBarangay = p.addrsPermBarangay;
                profile.addrsPermCityMun = p.addrsPermCityMun;
                profile.addrsPermProvince = p.addrsPermProvince;
                profile.addrsPermZip = p.addrsPermZip;

                Session["ProfileID"] = emp.idNo;
                string uID = Convert.ToInt16(emp.idNo).ToString("0000");
                string path = @"C:\DataFile\images\" + uID + ".jpg";
                if (System.IO.File.Exists(path))
                {
                    byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                    string imageBase64Data = Convert.ToBase64String(imageByteData);
                    string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);

                    //return Json(new { status = "success", profileData = profile, province = province, cityMun = cityMun, brgy = brgy, img = imageDataURL }, JsonRequestBehavior.AllowGet);

                    var jsonResult = Json(new { status = "success", profileData = profile, img = imageDataURL }, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;

                }
                else
                {
                    path = @"C:\DataFile\images\0000.jpg";
                    byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                    string imageBase64Data = Convert.ToBase64String(imageByteData);
                    string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                    return Json(new { status = "success", profileData = profile, img = imageDataURL }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public class TempPDSProfile
        {

            public string EIC { get; set; }
            public string idNo { get; set; }
            public string lastName { get; set; }
            public string firstName { get; set; }
            public string middleName { get; set; }
            public string extName { get; set; }
            public string fullNameLast { get; set; }
            public string fullNameFirst { get; set; }
            public string fullNameTitle { get; set; }
            public string namePrefix { get; set; }
            public string nameSuffix { get; set; }
            public Nullable<System.DateTime> birthDate { get; set; }
            public string birthPlace { get; set; }
            public string sex { get; set; }
            public string civilStatCode { get; set; }
            public Nullable<decimal> height { get; set; }
            public Nullable<decimal> weight { get; set; }
            public string bloodType { get; set; }
            public string GSISIDNo { get; set; }
            public string BPNo { get; set; }
            public string GSISPolicyNo { get; set; }
            public string HDMFNo { get; set; }
            public string HDMFMID { get; set; }
            public string PHICNo { get; set; }
            public string SSSNo { get; set; }
            public string TINNo { get; set; }


            public string telephoneNo { get; set; }
            public string mobileNo { get; set; }

            public string emailAddress { get; set; }

            public int citizenship { get; set; }
            public int citizenshipType { get; set; }
            public string citizenshipCountry { get; set; }


            public string addrsResHouseBlk { get; set; }
            public string addrsResStreet { get; set; }
            public string addrsResVillage { get; set; }
            public string addrsResBrgyCode { get; set; }
            public string addrsResBarangay { get; set; }
            public string addrsResCityMunCode { get; set; }
            public string addrsResCityMun { get; set; }
            public string addrsResProvCode { get; set; }
            public string addrsResProvince { get; set; }
            public string addrsResZip { get; set; }

            public string addrsPermHouseBlk { get; set; }
            public string addrsPermStreet { get; set; }
            public string addrsPermVillage { get; set; }
            public string addrsPermBrgyCode { get; set; }
            public string addrsPermBarangay { get; set; }
            public string addrsPermCityMunCode { get; set; }
            public string addrsPermCityMun { get; set; }
            public string addrsPermProvCode { get; set; }
            public string addrsPermProvince { get; set; }
            public string addrsPermZip { get; set; }

        }

        [HttpPost]
        public JsonResult UpdateProfile(TempPDSProfile data)
        {
            try
            {

                if (data.addrsResBrgyCode == null || data.addrsPermBrgyCode == null)
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }


                string uEIC = Session["_EIC"].ToString();
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == uEIC);

                emp.height = data.height;
                emp.weight = data.weight;
                emp.bloodType = data.bloodType;
                emp.telephoneNo = data.telephoneNo;
                emp.mobileNo = data.mobileNo;
                emp.emailAddress = data.emailAddress;

                tPDSProfile pro = db.tPDSProfiles.SingleOrDefault(e => e.EIC == uEIC);
                pro.citizenship = data.citizenship;
                pro.citizenshipType = data.citizenshipType;
                pro.citizenshipCountry = data.citizenshipCountry;

                if (data.citizenshipType == 0)
                {
                    TempPDSProfile tmp = new TempPDSProfile();
                    pro.citizenshipCountry = tmp.citizenshipCountry;
                }

                vRefAdrsBarangay brgyRes = db.vRefAdrsBarangays.Single(e => e.brgyCode == data.addrsResBrgyCode);
                vRefAdrsBarangay brgyPerm = db.vRefAdrsBarangays.Single(e => e.brgyCode == data.addrsPermBrgyCode);

                pro.addrsResHouseBlk = data.addrsResHouseBlk;
                pro.addrsResStreet = data.addrsResStreet;
                pro.addrsResVillage = data.addrsResVillage;
                pro.addrsResBrgyCode = data.addrsResBrgyCode;
                pro.addrsResZip = data.addrsResZip;

                pro.addrsPermHouseBlk = data.addrsPermHouseBlk;
                pro.addrsPermStreet = data.addrsPermStreet;
                pro.addrsPermVillage = data.addrsPermVillage;
                pro.addrsPermBrgyCode = data.addrsPermBrgyCode;
                pro.addrsResZip = data.addrsResZip;


                //pro.addrsResBarangay 

                //emp.houseBlockNoResAddress = data.houseBlockNoResAddress;
                //emp.streetResAddress = data.streetResAddress;
                //emp.subdivisionResAddress = data.subdivisionResAddress;
                //emp.brgyResAddress = data.brgyResAddress;
                //emp.cityMunResAddress = data.cityMunResAddress;
                //emp.provinceResAddress = data.provinceResAddress;
                //emp.ZIPResAddress = data.ZIPResAddress;

                //emp.houseBlockNoPermAddress = data.houseBlockNoPermAddress;
                //emp.streetPermAddress = data.streetPermAddress;
                //emp.subdivisionPermAddress = data.subdivisionPermAddress;
                //emp.brgyPermAddress = data.brgyPermAddress;
                //emp.cityMunPermAddress = data.cityMunPermAddress;
                //emp.provincePermAddress = data.provincePermAddress;
                //emp.ZIPPermAddress = data.ZIPPermAddress;



                db.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult FamilyBackground()
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tPDSProfile p = db.tPDSProfiles.SingleOrDefault(e => e.EIC == uEIC);
                IEnumerable<tPDSChildren> children = db.tPDSChildrens.Where(e => e.EIC == uEIC && e.birthDate != null).OrderBy(o => o.birthDate).ToList();
                if (p == null)
                {
                    tPDSProfile pds = new tPDSProfile();
                    pds.EIC = uEIC;
                    db.SaveChanges();
                }
                return Json(new { status = "success", famData = p, children = children }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult UpdateFamilyBackgrnd(tPDSProfile data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tPDSProfile p = db.tPDSProfiles.Single(e => e.EIC == uEIC);
                p.spouseOccupation = data.spouseOccupation;
                p.spouseEmployeerName = data.spouseEmployeerName;
                p.spouseEmployeerAddrs = data.spouseEmployeerAddrs;
                p.spouseEmployeerTelNo = data.spouseEmployeerTelNo;
                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult SavePDSChild(tPDSChildren data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tPDSChildren c = new tPDSChildren();
                c.EIC = uEIC;
                c.childName = data.childName;
                c.birthDate = data.birthDate;
                c.tag = 0;
                db.tPDSChildrens.Add(c);
                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpPost]
        public JsonResult EducationBackground()
        {
            try
            {
                List<TempEducation> educ = MyEducationList();
                return Json(new { status = "success", education = educ }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public class TempEducation
        {
            public string controlNo { get; set; }
            public string educLevelCode { get; set; }
            public string educLevelName { get; set; }
            public string schoolName { get; set; }
            public string degree { get; set; }
            public int? periodYearFrom { get; set; }
            public int? periodYearTo { get; set; }
            public string highestLevel { get; set; }
            public string yearGraduated { get; set; }
            public string honorsReceived { get; set; }
            public int? isVerified { get; set; }
        }

        private List<TempEducation> MyEducationList()
        {
            string uEIC = Session["_EIC"].ToString();
            List<TempEducation> myList = (from e in db.tPDSEducations
                                          join l in db.tPDSEducationLevels on e.educLevelCode equals l.educLevelCode
                                          where e.EIC == uEIC
                                          select new TempEducation()
                                          {
                                              controlNo = e.controlNo,
                                              educLevelName = l.educLevelName,
                                              educLevelCode = e.educLevelCode,
                                              schoolName = e.schoolName,
                                              degree = e.degree,
                                              periodYearFrom = e.periodYearFrom,
                                              periodYearTo = e.periodYearTo,
                                              highestLevel = e.highestLevel,
                                              yearGraduated = e.yearGraduated,
                                              honorsReceived = e.honorsReceived,
                                              isVerified = e.isVerified
                                          }).OrderBy(o => o.educLevelCode).ThenBy(o => o.periodYearFrom).ToList();
            return myList;
        }

        [HttpPost]
        public JsonResult SaveEducation(tPDSEducation data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                DateTime dt = DateTime.Now;
                string tmp = "T" + dt.ToString("yyMMddHHmm") + "EDUC" + uEIC.Substring(0, 5) + dt.ToString("ssfff");

                tPDSEducation n = new tPDSEducation();
                n.controlNo = tmp;
                n.EIC = uEIC;
                n.educLevelCode = data.educLevelCode;
                n.schoolName = data.schoolName;
                n.degree = data.degree;
                n.periodYearFrom = data.periodYearFrom;
                n.periodYearTo = data.periodYearTo;
                n.highestLevel = data.highestLevel;
                n.yearGraduated = data.yearGraduated;
                n.honorsReceived = data.honorsReceived;
                n.isVerified = 0;
                n.transDT = DateTime.Now;
                db.tPDSEducations.Add(n);
                db.SaveChanges();

                List<TempEducation> educ = MyEducationList();

                return Json(new { status = "success", education = educ }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateEducationData(tPDSEducation data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tPDSEducation educ = db.tPDSEducations.Single(e => e.EIC == uEIC && e.controlNo == data.controlNo);
                if (educ.isVerified >= 1)
                {
                    return Json(new { status = "Unable to update record!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    educ.educLevelCode = data.educLevelCode;
                    educ.schoolName = data.schoolName;
                    educ.degree = data.degree;
                    educ.periodYearFrom = data.periodYearFrom;
                    educ.periodYearTo = data.periodYearTo;
                    educ.highestLevel = data.highestLevel;
                    educ.yearGraduated = data.yearGraduated;
                    educ.honorsReceived = data.honorsReceived;
                    educ.isVerified = 0;
                    db.SaveChanges();
                }

                List<TempEducation> myList = MyEducationList();
                return Json(new { status = "success", education = myList }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EducationDeleteData(tPDSEducation data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tPDSEducation educ = db.tPDSEducations.Single(e => e.EIC == uEIC && e.controlNo == data.controlNo);
                if (educ.isVerified >= 1)
                {
                    return Json(new { status = "Unable to delete record!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.tPDSEducations.Remove(educ);
                    db.SaveChanges();
                    List<TempEducation> educList = MyEducationList();
                    return Json(new { status = "success", education = educList }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        //ELIGBIBILITY
        [HttpPost]
        public JsonResult GetEligibility(int tag)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();

                List<TempEligibility> elig = EligibilityList(uEIC);

                if (tag == 0)
                {
                    var eligTypeList = (from e in db.tRSPEligibilities
                                        select new
                                        {
                                            e.eligibilityCode,
                                            e.eligibilityName
                                        }).ToList();
                    return Json(new { status = "success", eligibility = elig, eligTypeList = eligTypeList }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = "success", eligibility = elig }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public class TempEligibility
        {
            public string controlNo { get; set; }
            public string eligibilityName { get; set; }
            public string rating { get; set; }
            public DateTime? examDate { get; set; }
            public string examPlace { get; set; }
            public string licenseNo { get; set; }
            public DateTime? validityDate { get; set; }
            public int? isVerified { get; set; }
            public string remarks { get; set; }
        }

        private List<TempEligibility> EligibilityList(string uEIC)
        {

            List<TempEligibility> elig = (from e in db.tPDSEligibilities
                                          join f in db.tRSPEligibilities on e.eligibilityCode equals f.eligibilityCode
                                          where e.EIC == uEIC
                                          select new TempEligibility()
                        {
                            controlNo = e.controlNo,
                            eligibilityName = f.eligibilityName,
                            rating = e.rating,
                            examDate = e.examDate,
                            examPlace = e.examPlace,
                            licenseNo = e.licenseNo,
                            validityDate = e.validityDate,
                            isVerified = e.isVerified,
                            remarks = e.remarks
                        }).ToList();

            return elig;
        }

        public JsonResult SubmitEligibility(tPDSEligibility data)
        {
            try
            {
                DateTime dt = DateTime.Now;
                string uEIC = Session["_EIC"].ToString();
                string tmp = "T" + dt.ToString("yyMMddHHmm") + "ELIG" + uEIC.Substring(0, 5) + dt.ToString("ssfff");

                tPDSEligibility e = new tPDSEligibility();
                e.controlNo = tmp;
                e.eligibilityCode = data.eligibilityCode;
                e.rating = data.rating;
                e.examDate = data.examDate;
                e.examPlace = data.examPlace;
                e.validityDate = data.validityDate;
                e.EIC = uEIC;
                e.transDT = DateTime.Now;
                e.isVerified = 0;
                e.remarks = "";
                db.tPDSEligibilities.Add(e);
                db.SaveChanges();

                List<TempEligibility> elig = EligibilityList(uEIC);

                return Json(new { status = "success", eligibility = elig }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetWorkExperience()
        {
            try
            {
                List<TempPDSWorkExperience> workExpr = MyWorkExperienceSheet();
                return Json(new { status = "success", workExperience = workExpr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public List<TempPDSWorkExperience> MyWorkExperienceSheet()
        {
            string uEIC = Session["_EIC"].ToString();
            List<TempPDSWorkExperience> workExpr = (from w in db.tPDSWorkExperiences
                                                    where w.EIC == uEIC
                                                    select new TempPDSWorkExperience()
                                                    {
                                                        controlNo = w.controlNo,
                                                        dateFrom = w.dateFrom,
                                                        dateTo = w.dateTo,
                                                        positionTitle = w.positionTitle,
                                                        agencyCompany = w.agencyCompany,
                                                        monthlySalary = w.monthlySalary,
                                                        sgStep = w.salaryGrade >= 1 ? w.salaryGrade.ToString() + "/" + w.step.ToString() : "N/A",
                                                        appointmentStatus = w.appointmentStatus,
                                                        isGovtService = w.isGovtService,
                                                        isVerified = w.isVerified
                                                    }).OrderByDescending(o => o.dateFrom).ToList();

            return workExpr;
        }

        public class TempPDSWorkExperience
        {
            public string controlNo { get; set; }
            public DateTime? dateFrom { get; set; }
            public DateTime? dateTo { get; set; }
            public string positionTitle { get; set; }
            public string agencyCompany { get; set; }
            public decimal? monthlySalary { get; set; }
            public string sgStep { get; set; }
            public string appointmentStatus { get; set; }
            public int? isGovtService { get; set; }
            public int? isVerified { get; set; }

        }

        [HttpPost]
        public JsonResult SaveWorkExperience(TempPDSWorkExperience data)
        {
            try
            {
                DateTime dt = DateTime.Now;
                string uEIC = Session["_EIC"].ToString();
                string tmp = "T" + dt.ToString("yyMMddHHmm") + "EXPR" + uEIC.Substring(0, 5) + dt.ToString("ssfff");

                int tempSalaryGrade = 0;
                int tempStep = 0;
                if (data.isGovtService == 1)
                {
                    data.sgStep = data.sgStep.Trim();
                    string[] split = data.sgStep.Split('/');
                    tempSalaryGrade = Convert.ToInt16(split[0]);
                    tempStep = Convert.ToInt16(split[1]);
                }

                tPDSWorkExperience exp = new tPDSWorkExperience();
                exp.controlNo = tmp;
                exp.EIC = uEIC;
                exp.dateFrom = data.dateFrom;
                exp.dateTo = data.dateTo;
                exp.positionTitle = data.positionTitle;
                exp.agencyCompany = data.agencyCompany;
                exp.monthlySalary = data.monthlySalary;
                exp.salaryGrade = tempSalaryGrade;
                exp.step = tempStep;
                exp.appointmentStatus = data.appointmentStatus;
                exp.isGovtService = data.isGovtService;
                exp.transDT = DateTime.Now;
                exp.isVerified = 0;
                db.tPDSWorkExperiences.Add(exp);
                db.SaveChanges();

                List<TempPDSWorkExperience> workExpr = MyWorkExperienceSheet();

                return Json(new { status = "success", workExperience = workExpr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult UpdateWorkExperience(TempPDSWorkExperience data)
        {
            try
            {

                DateTime dt = DateTime.Now;
                string uEIC = Session["_EIC"].ToString();
                //string tmp = "T" + dt.ToString("yyMMddHHmm") + "EXPR" + uEIC.Substring(0, 5) + dt.ToString("ssfff");
                tPDSWorkExperience exp = db.tPDSWorkExperiences.Single(e => e.EIC == uEIC && e.controlNo == data.controlNo);
                if (exp.isVerified >= 1)
                {
                    return Json(new { status = "Unable to update record!" }, JsonRequestBehavior.AllowGet);
                }

                int tempSalaryGrade = 0; int tempStep = 0;
                if (data.isGovtService == 1)
                {
                    data.sgStep = data.sgStep.Trim();
                    string[] split = data.sgStep.Split('/');
                    tempSalaryGrade = Convert.ToInt16(split[0]);
                    tempStep = Convert.ToInt16(split[1]);
                }

                exp.dateFrom = data.dateFrom;
                exp.dateTo = data.dateTo;
                exp.positionTitle = data.positionTitle;
                exp.agencyCompany = data.agencyCompany;
                exp.monthlySalary = data.monthlySalary;
                exp.salaryGrade = tempSalaryGrade;
                exp.step = tempStep;
                exp.appointmentStatus = data.appointmentStatus;
                exp.isGovtService = data.isGovtService;
                exp.transDT = DateTime.Now;
                exp.isVerified = 0;
                db.SaveChanges();

                List<TempPDSWorkExperience> workExpr = MyWorkExperienceSheet();
                return Json(new { status = "success", workExperience = workExpr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult DeleteWorkExperience(TempPDSWorkExperience data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tPDSWorkExperience exp = db.tPDSWorkExperiences.Single(e => e.EIC == uEIC && e.controlNo == data.controlNo);
                if (exp.isVerified >= 1)
                {
                    return Json(new { status = "Unable to delete record!" }, JsonRequestBehavior.AllowGet);
                }
                db.tPDSWorkExperiences.Remove(exp);
                db.SaveChanges();
                List<TempPDSWorkExperience> workExpr = MyWorkExperienceSheet();
                return Json(new { status = "success", workExperience = workExpr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////
        //TRAINING / Learning & Development Intervention (LDI)


        [HttpPost]
        public JsonResult GetPDSTrainings()
        {
            try
            {
                IEnumerable<tPDSTraining> myList = TrainingList();
                return Json(new { status = "success", trainings = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        private IEnumerable<tPDSTraining> TrainingList()
        {
            string uEIC = Session["_EIC"].ToString();
            IEnumerable<tPDSTraining> list = db.tPDSTrainings.Where(e => e.EIC == uEIC).OrderBy(o => o.dateFrom).ToList();
            return list;
        }


        [HttpPost]
        public JsonResult SaveLDITraining(tPDSTraining data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                DateTime dt = DateTime.Now;
                string tmp = "T" + dt.ToString("yyMMddHHmm") + "LNDI" + uEIC.Substring(0, 5) + dt.ToString("ssfff");

                tPDSTraining t = new tPDSTraining();
                t.controlNo = tmp;
                t.EIC = uEIC;
                t.trainingTitle = data.trainingTitle;
                t.dateFrom = data.dateFrom;
                t.dateTo = data.dateTo;
                t.hoursNo = data.hoursNo;
                t.trainingTypeCode = data.trainingTypeCode;
                t.conductedBy = data.conductedBy;
                t.transDT = DateTime.Now;
                t.isVerified = 0;
                db.tPDSTrainings.Add(t);
                db.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetPDSOtherInfo()
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();

                DateTime dt = DateTime.Now;
                IEnumerable<tPDSReference> refData = db.tPDSReferences.Where(e => e.EIC == uEIC).OrderBy(o => o.tag).ToList();

                var pdsInfo = db.tPDSProfiles.Select(e => new
                {
                    e.EIC,
                    e.infoDegreeThird,
                    e.infoDegreeFourth,
                    e.infoDegreeFourthDetails,
                    e.infoAdminOffense,
                    e.infoAdminOffenseDetails,
                    e.infoCriminalCharge,
                    e.infoCriminalChargeFileDate,
                    e.infoCriminalChageCaseStatus,
                    e.infoConvicted,
                    e.infoConvictedDetails,
                    e.infoHasSeparatedService,
                    e.infoHasSeparatedServiceDetails,
                    e.infoIsCandidate,
                    e.infoIsCandidateDetails,
                    e.infoHasGovtResigned,
                    e.infoHasGovtResignedDetails,
                    e.infoHasImmigrant,
                    e.infoHasImmigrantDetails,
                    e.infoIndigenous,
                    e.infoIndigenousDetails,
                    e.infoPWD,
                    e.infoPWDDetails,
                    e.infoSoloParent,
                    e.infoSoloParentDetails,
                    e.govtIssuedId,
                    e.govtIssuedIdNo,
                    e.govtIssuedDatePlace
                }).Single(e => e.EIC == uEIC);


                List<tPDSReference> charRef = new List<tPDSReference>();

                if (refData.Count() == 0)
                {
                    for (int counter = 1; 3 >= counter; counter++)
                    {
                        string tmp = "T" + dt.ToString("yyMMddHHmm") + "CHAR" + uEIC.Substring(0, 3) + dt.ToString("ssfff") + counter.ToString("00");
                        tPDSReference r = new tPDSReference();
                        r.controlNo = tmp;
                        r.name = "";
                        r.address = "";
                        r.telNo = "";
                        r.EIC = uEIC;
                        r.tag = counter;
                        db.tPDSReferences.Add(r);
                        charRef.Add(r);
                    }
                    db.SaveChanges();
                }
                else
                {
                    foreach (tPDSReference item in refData)
                    {
                        charRef.Add(new tPDSReference()
                        {
                            controlNo = item.controlNo,
                            name = item.name,
                            address = item.address,
                            telNo = item.telNo,
                            tag = item.tag
                        });
                    }
                }
                return Json(new { status = "success", pdsInfo = pdsInfo, charRef = charRef }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateCharReferenceData(tPDSReference data)
        {
            try
            {
                string uEIC = Session["_EIC"].ToString();
                tPDSReference r = db.tPDSReferences.SingleOrDefault(e => e.controlNo == data.controlNo && e.EIC == uEIC);
                if (r == null)
                {
                    return Json(new { status = "Invalid data!" }, JsonRequestBehavior.AllowGet);
                }
                r.name = data.name;
                r.address = data.address;
                r.telNo = data.telNo;
                db.SaveChanges();

                var charRef = db.tPDSReferences.Select(e => new
                {
                    e.controlNo,
                    e.name,
                    e.address,
                    e.telNo,
                    e.tag
                }).OrderBy(o => o.tag).ToList();


                return Json(new { status = "success", charRef = charRef }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateOtherInformation(tPDSProfile data)
        {
            try
            {

                int hasError = HasOtherInfoError(data);
                if (hasError == 1)
                {
                    return Json(new { status = "Please fill-up the required fields!" }, JsonRequestBehavior.AllowGet);
                }

                string uEIC = Session["_EIC"].ToString();
                tPDSProfile p = db.tPDSProfiles.Single(e => e.EIC == uEIC);

                p.infoDegreeThird = data.infoDegreeThird;

                p.infoDegreeFourth = data.infoDegreeFourth;
                p.infoDegreeFourthDetails = "";
                if (data.infoDegreeFourth == 1)
                {
                    p.infoDegreeFourthDetails = data.infoDegreeFourthDetails;
                }

                p.infoAdminOffense = data.infoAdminOffense;
                p.infoAdminOffenseDetails = "";
                if (data.infoAdminOffense == 1)
                {
                    p.infoAdminOffenseDetails = data.infoAdminOffenseDetails;
                }

                p.infoCriminalCharge = data.infoCriminalCharge;
                p.infoCriminalChargeFileDate = null;
                p.infoCriminalChageCaseStatus = "";
                if (data.infoCriminalCharge == 1)
                {
                    p.infoCriminalChargeFileDate = Convert.ToDateTime(data.infoCriminalChargeFileDate);
                    p.infoCriminalChageCaseStatus = data.infoCriminalChageCaseStatus;
                }

                p.infoConvicted = data.infoConvicted;
                p.infoConvictedDetails = "";
                if (data.infoConvicted == 1)
                {
                    p.infoConvictedDetails = data.infoConvictedDetails;
                }

                p.infoHasSeparatedService = data.infoHasSeparatedService;
                p.infoHasSeparatedServiceDetails = "";
                if (data.infoHasSeparatedService == 1)
                {
                    p.infoHasSeparatedServiceDetails = data.infoHasSeparatedServiceDetails;
                }

                p.infoIsCandidate = data.infoIsCandidate;
                p.infoIsCandidateDetails = "";
                if (data.infoIsCandidate == 1)
                {
                    p.infoIsCandidateDetails = data.infoIsCandidateDetails;
                }

                p.infoHasGovtResigned = data.infoHasGovtResigned;
                p.infoHasGovtResignedDetails = "";
                if (data.infoHasGovtResigned == 1)
                {
                    p.infoHasGovtResignedDetails = data.infoHasGovtResignedDetails;
                }

                p.infoHasImmigrant = data.infoHasImmigrant;
                p.infoHasImmigrantDetails = "";
                if (data.infoHasImmigrant == 1)
                {
                    p.infoHasImmigrantDetails = data.infoHasImmigrantDetails;
                }

                p.infoIndigenous = data.infoIndigenous;
                p.infoIndigenousDetails = "";
                if (data.infoIndigenous == 1)
                {
                    p.infoIndigenousDetails = data.infoIndigenousDetails;
                }

                p.infoPWD = data.infoPWD;
                p.infoPWDDetails = "";
                if (data.infoPWD == 1)
                {
                    p.infoPWDDetails = data.infoPWDDetails;
                }

                p.infoSoloParent = data.infoSoloParent;
                p.infoSoloParentDetails = "";
                if (data.infoSoloParent == 1)
                {
                    p.infoSoloParentDetails = data.infoSoloParentDetails;
                }

                p.govtIssuedId = data.govtIssuedId;
                p.govtIssuedIdNo = data.govtIssuedIdNo;
                p.govtIssuedDatePlace = data.govtIssuedDatePlace;

                db.SaveChanges();


                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        private int HasOtherInfoError(tPDSProfile data)
        {
            int res = 0;

            try
            {
                //FOURTH DEGREE
                if (data.infoDegreeFourth == 1)
                {
                    if (data.infoDegreeFourthDetails == null)
                    {
                        return 1;
                    }
                    if (data.infoDegreeFourthDetails.Trim() == "")
                    {
                        return 1;
                    }
                }

                //ADMIN OFFENSE
                if (data.infoAdminOffense == 1)
                {
                    if (data.infoAdminOffenseDetails == null)
                    {
                        return 1;
                    }
                    if (data.infoAdminOffenseDetails.Trim() == "")
                    {
                        return 1;
                    }
                }

                //CRIMINALLY CHARGED
                if (data.infoCriminalCharge == 1)
                {
                    if (data.infoCriminalChargeFileDate == null)
                    {
                        return 1;
                    }
                    DateTime tmpDate = Convert.ToDateTime(data.infoCriminalChargeFileDate);
                    if (tmpDate.Year <= 100)
                    {
                        return 1;
                    }
                    if (data.infoCriminalChageCaseStatus == null)
                    {
                        return 1;
                    }
                    if (data.infoCriminalChageCaseStatus.Trim() == "")
                    {
                        return 1;
                    }
                }
                 //CONVICTED
                if (data.infoConvicted == 1)
                {
                    if (data.infoConvictedDetails == null)
                    {
                        return 1;
                    }
                    if (data.infoConvictedDetails.Trim() == "")
                    {
                        return 1;
                    }
                }

                //SEPARATED
                if (data.infoHasSeparatedService == 1)
                {
                    if (data.infoHasSeparatedServiceDetails == null)
                    {
                        return 1;
                    }
                    if (data.infoHasSeparatedServiceDetails.Trim() == "")
                    {
                        return 1;
                    }
                }

                //INFO CANDIDATE
                if (data.infoIsCandidate == 1)
                {
                    if (data.infoIsCandidateDetails == null)
                    {
                        return 1;
                    }
                    if (data.infoIsCandidateDetails.Trim() == "")
                    {
                        return 1;
                    }
                }

                //INFO RESIGNED
                if (data.infoHasGovtResigned == 1)
                {
                    if (data.infoHasGovtResignedDetails == null)
                    {
                        return 1;
                    }
                    if (data.infoHasGovtResignedDetails.Trim() == "")
                    {
                        return 1;
                    }
                }

                //IMMIGRANT 
                if (data.infoHasImmigrant == 1)
                {
                    if (data.infoHasImmigrantDetails == null)
                    {
                        return 1;
                    }
                    if (data.infoHasImmigrantDetails.Trim() == "")
                    {
                        return 1;
                    }
                }
                
                //INDIGENOUS 
                if (data.infoIndigenous == 1)
                {
                    if (data.infoIndigenousDetails == null)
                    {
                        return 1;
                    }
                    if (data.infoIndigenousDetails.Trim() == "")
                    {
                        return 1;
                    }
                }

                //INFO IS PWD 
                if (data.infoPWD == 1)
                {
                    if (data.infoPWDDetails == null)
                    {
                        return 1;
                    }
                    if (data.infoPWDDetails.Trim() == "")
                    {
                        return 1;
                    }
                }

                //INFO IS PWD 
                if (data.infoSoloParent == 1)
                {
                    if (data.infoSoloParentDetails == null)
                    {
                        return 1;
                    }
                    if (data.infoSoloParentDetails.Trim() == "")
                    {
                        return 1;
                    }
                }
                
                //SOLO PARENT
                if (data.infoSoloParent == 1)
                {
                    if (data.infoSoloParentDetails == null)
                    {
                        return 1;
                    }
                    if (data.infoSoloParentDetails.Trim() == "")
                    {
                        return 1;
                    }
                }

                if (data.govtIssuedId == null)
                {
                    return 1;
                }
                
                if (data.govtIssuedId.Trim() == "")
                {
                    return 1;
                }

                if (data.govtIssuedIdNo == null)
                {
                    return 1;
                }

                if (data.govtIssuedIdNo.Trim() == "")
                {
                    return 1;
                }

                if (data.govtIssuedDatePlace == null)
                {
                    return 1;
                }

                if (data.govtIssuedDatePlace.Trim() == "")
                {
                    return 1;
                }

                return res;
                 
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return 1;
            }




        }

        //PRINT PDS
        ////////////////////////////////////////////////////////////////////////////////////

        [HttpPost]
        public JsonResult PrintMyPDS(string id, int code)
        {
            try
            {
                id = Session["_EIC"].ToString();
                string reportPage = "";

                tRSPEmployee emp = db.tRSPEmployees.SingleOrDefault(e => e.EIC == id);

                if (emp == null)
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }

                if (code == 1)
                {
                    reportPage = "PDSPAGE1";
                    //CHILDREN COUNTER 
                    IEnumerable<tPDSEducation> education = db.tPDSEducations.Where(e => e.EIC == id && e.isVerified == 1).OrderBy(o => o.periodYearFrom).ToList();
                    if (education.Count() > 6)
                    {
                        IEnumerable<tPDSEducation> educSkip6 = education.Skip(6).ToList();
                        foreach (tPDSEducation item in educSkip6)
                        {
                            tPDSEducation educUpdate = db.tPDSEducations.Single(e => e.controlNo == item.controlNo);
                            educUpdate.pageNo = 2;
                            db.SaveChanges();
                        }
                    }
                    //EDUCATION COUNTER
                    IEnumerable<tPDSChildren> children = db.tPDSChildrens.Where(e => e.EIC == id).OrderBy(o => o.birthDate).ToList();
                    int childCount = children.Where(e => e.isVerified == 1).Count();
                    int childDecoy = children.Where(e => e.tag == 0).Count();
                    if (childCount <= 14)
                    {
                        if (childCount < 14)
                        {
                            int loopCount = 14 - (childCount + childDecoy);
                            //DELETE ALL TAG 0                             
                            for (int counter = 0; loopCount > counter; counter++)
                            {
                                tPDSChildren child = new tPDSChildren();
                                child.EIC = id;
                                child.childName = "";
                                child.tag = 0;
                                child.pageNo = 1;
                                db.tPDSChildrens.Add(child);
                                db.SaveChanges();
                            }

                        }
                    }
                    else if (childCount > 14)
                    {
                        IEnumerable<tPDSChildren> childNextPage = children.Where(e => e.tag == 1).OrderBy(o => o.birthDate).ToList();
                        childNextPage = childNextPage.Skip(14).ToList();
                        foreach (tPDSChildren item in childNextPage)
                        {
                            tPDSChildren childNxt = db.tPDSChildrens.Single(e => e.recNo == item.recNo);
                            childNxt.pageNo = 2;
                            db.SaveChanges();
                        }

                    }

                }
                else if (code == 2)
                {
                    reportPage = "PDSPAGE2";
                    IEnumerable<tPDSEligibility> elig = db.tPDSEligibilities.Where(e => e.EIC == id).OrderBy(o => o.controlNo).ToList();
                    int eligDecoy = elig.Where(e => e.isVerified == 2).Count();
                    int eligCount = elig.Where(e => e.isVerified == 1).Count();

                    //ELIGIBILITY
                    if (eligCount <= 6)
                    {
                        DateTime dt = DateTime.Now;
                        int loopCount = 6 - (eligCount + eligDecoy);
                        for (int counter = 0; loopCount > counter; counter++)
                        {
                            string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "ELIG" + dt.ToString("ssfff") + counter.ToString("00");
                            tPDSEligibility e = new tPDSEligibility();
                            e.EIC = id;
                            e.controlNo = tmpCode;
                            e.isVerified = 2;
                            db.tPDSEligibilities.Add(e);
                            db.SaveChanges();
                        }
                    }
                    else
                    {

                    }

                    //WORK EXPERIENCE
                    IEnumerable<tPDSWorkExperience> xper = db.tPDSWorkExperiences.Where(e => e.EIC == id).ToList();
                    IEnumerable<tPDSWorkExperience> workExper = xper.Where(e => e.isVerified == 1).ToList();
                    int xperCount = workExper.Where(e => e.isVerified == 1).Count();
                    int xperDecoy = xper.Where(e => e.isVerified == 2).Count();
                    if (xperCount <= 30)
                    {
                        //CHECK FOR PAGE 2
                        int tmp = workExper.Where(e => e.pageNo >= 2).Count();
                        if (tmp >= 1)
                        {
                            foreach (tPDSWorkExperience item in workExper)
                            {
                                tPDSWorkExperience myItem = db.tPDSWorkExperiences.Single(e => e.controlNo == item.controlNo);
                                myItem.pageNo = 1;
                                db.SaveChanges();
                            }
                        }
                        //BLANK ROWS
                        DateTime dt = DateTime.Now;
                        int loopCount = 30 - (xperCount + xperDecoy);
                        for (int counter = 0; loopCount > counter; counter++)
                        {
                            string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "ELIG" + dt.ToString("ssfff") + counter.ToString("00");
                            tPDSWorkExperience workExp = new tPDSWorkExperience();
                            workExp.controlNo = tmpCode;
                            workExp.isVerified = 2;
                            workExp.EIC = id;
                            db.tPDSWorkExperiences.Add(workExp);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        //IEnumerable<tPDSChildren> childNextPage = children.Where(e => e.tag == 1).OrderBy(o => o.birthDate).ToList();
                        int pageCounter = 1;
                        int myCounter = 0;
                        foreach (tPDSWorkExperience item in workExper)
                        {
                            myCounter = myCounter + 1;
                            tPDSWorkExperience myItem = db.tPDSWorkExperiences.Single(e => e.controlNo == item.controlNo);
                            myItem.pageNo = pageCounter;
                            db.SaveChanges();
                            if (myCounter >= 30)
                            {
                                pageCounter = pageCounter + 1;
                                myCounter = 0;
                            }
                        }
                    }

                }
                else if (code == 3)
                {
                    DateTime dt = DateTime.Now;
                    reportPage = "PDSPAGE3";

                    // VOLUNTAY WORKS ************************** 
                    IEnumerable<tPDSVoluntaryWork> list = db.tPDSVoluntaryWorks.Where(e => e.EIC == id).ToList();
                    IEnumerable<tPDSVoluntaryWork> workList = list.Where(e => e.isVerified == 1).ToList();
                    int decoyCount = list.Where(e => e.isVerified >= 2).Count();
                    int count = workList.Count();

                    if (count <= 7)
                    {
                        int loopCount = 7 - (count + decoyCount);
                        for (int counter = 0; loopCount > counter; counter++)
                        {
                            string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "VWRK" + dt.ToString("ssfff") + counter.ToString("00");
                            tPDSVoluntaryWork w = new tPDSVoluntaryWork();
                            w.controlNo = tmpCode;
                            w.EIC = id;
                            w.isVerified = 2;
                            w.pageNo = 1;
                            db.tPDSVoluntaryWorks.Add(w);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        //CHECK PAGE ITEM > 7
                        int pageTmpCounter = workList.Where(e => e.pageNo == 1).Count();
                        if (pageTmpCounter > 7)
                        {
                            int tmpCounter = 0;
                            int tmpPageCounter = 1;
                            foreach (tPDSVoluntaryWork item in workList)
                            {
                                tmpCounter = tmpCounter + 1;
                                tPDSVoluntaryWork tmp = db.tPDSVoluntaryWorks.Single(e => e.controlNo == item.controlNo);
                                tmp.pageNo = tmpPageCounter;
                                db.SaveChanges();
                                if (tmpCounter >= 7)
                                {
                                    tmpPageCounter = tmpPageCounter + 1;
                                    tmpCounter = 0;
                                }
                            }
                        }
                    }

                    // TRAININGS (LDI) ************************** 
                    IEnumerable<tPDSTraining> tempList = db.tPDSTrainings.Where(e => e.EIC == id).ToList();
                    IEnumerable<tPDSTraining> trainings = tempList.Where(e => e.isVerified == 1).ToList();

                    int trainingDecoy = tempList.Where(e => e.isVerified >= 2).Count();
                    int trainingCount = trainings.Count();

                    if (trainingCount <= 20)
                    {
                        int loopCount = 20 - (trainingCount + trainingDecoy);
                        for (int counter = 0; loopCount > counter; counter++)
                        {
                            string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "TRAI" + dt.ToString("ssfff") + counter.ToString("00");
                            tPDSTraining n = new tPDSTraining();
                            n.EIC = id;
                            n.controlNo = tmpCode;
                            n.pageNo = 1;
                            n.isVerified = 3;
                            db.tPDSTrainings.Add(n);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        int pageTmpCounter = trainings.Where(e => e.pageNo == 1).Count();
                        if (pageTmpCounter > 20)
                        {
                            int tmpCounter = 0;
                            int tmpPageCounter = 1;
                            foreach (tPDSTraining item in trainings)
                            {
                                tPDSTraining t = db.tPDSTrainings.Single(e => e.controlNo == item.controlNo);
                                t.pageNo = tmpPageCounter;
                                db.SaveChanges();
                                tmpCounter = tmpCounter + 1;
                                if (tmpCounter >= 20)
                                {
                                    tmpPageCounter = tmpPageCounter + 1;
                                    tmpCounter = 0;
                                }
                            }
                        }
                    }



                    // -------------------------------------------------------------------------------------------------
                    IEnumerable<tPDSSpecialSkill> otherInfoList = db.tPDSSpecialSkills.Where(e => e.EIC == id).ToList();

                    // SPECIAL SKILLS  ************************** 
                    IEnumerable<tPDSSpecialSkill> skills = otherInfoList.Where(e => e.type == "SKILLS").ToList();
                    IEnumerable<tPDSSpecialSkill> mySkills = skills.Where(e => e.tag == 1).ToList();

                    int skillsCount = mySkills.Count();

                    if (skillsCount <= 6)
                    {
                        int skillsDecoy = skills.Where(e => e.tag > 1).Count();
                        int loopCount = 6 - (skillsCount + skillsDecoy);
                        for (int counter = 0; loopCount > counter; counter++)
                        {
                            string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "SKLL" + dt.ToString("ssfff") + counter.ToString("00");
                            tPDSSpecialSkill ns = new tPDSSpecialSkill();
                            ns.controlNo = tmpCode;
                            ns.EIC = id;
                            ns.tag = 3;
                            ns.type = "SKILLS";
                            ns.pageNo = 1;
                            db.tPDSSpecialSkills.Add(ns);
                            db.SaveChanges();
                        }

                        //DELETE TEXTRA DECOY
                        int delCounter = 6 - skillsCount;
                        IEnumerable<tPDSSpecialSkill> skillDel = otherInfoList.Where(e => e.type == "SKILLS" && e.tag > 1).ToList();
                        if (skillDel.Count() > delCounter)
                        {
                            skillDel = skillDel.Skip(delCounter).ToList();
                            foreach (tPDSSpecialSkill delItem in skillDel)
                            {
                                tPDSSpecialSkill d = db.tPDSSpecialSkills.Single(e => e.controlNo == delItem.controlNo);
                                db.tPDSSpecialSkills.Remove(d);
                                db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        int skillsPageCounter = mySkills.Where(e => e.tag == 1).Count();
                        if (skillsPageCounter != 6)
                        {
                            //RE PAGING SETUP
                            int tmpCounter = 0;
                            int tmpPageCounter = 1;
                            foreach (tPDSSpecialSkill item in mySkills)
                            {
                                tPDSSpecialSkill myData = db.tPDSSpecialSkills.Single(e => e.controlNo == item.controlNo);
                                myData.pageNo = tmpPageCounter;
                                db.SaveChanges();
                                tmpCounter = tmpCounter + 1;
                                if (tmpCounter >= 6)
                                {
                                    tmpCounter = 0;
                                    tmpPageCounter = tmpPageCounter + 1;
                                }
                            }
                        }
                        int skillsDel = skills.Where(e => e.tag > 1).Count();
                        if (skillsDel >= 1)
                        {
                            //DELETE DECOY SKILLS
                            IEnumerable<tPDSSpecialSkill> skillDel = otherInfoList.Where(e => e.type == "SKILLS" && e.tag > 1).ToList();
                            foreach (tPDSSpecialSkill delItem in skillDel)
                            {
                                tPDSSpecialSkill d = db.tPDSSpecialSkills.Single(e => e.controlNo == delItem.controlNo);
                                db.tPDSSpecialSkills.Remove(d);
                                db.SaveChanges();
                            }
                        }
                    }

                    // RECOGNITION  ***************************** 
                    IEnumerable<tPDSSpecialSkill> recognition = otherInfoList.Where(e => e.type == "RECOG").ToList();
                    IEnumerable<tPDSSpecialSkill> recog = recognition.Where(e => e.tag == 1).ToList();
                    int recogCount = recog.Count();
                    if (recogCount <= 6)
                    {
                        int recogDecoy = recognition.Where(e => e.tag > 1).Count();
                        int loopCount = 6 - (recogCount + recogDecoy);
                        for (int counter = 0; loopCount > counter; counter++)
                        {
                            string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "RECO" + dt.ToString("ssfff") + counter.ToString("00");
                            tPDSSpecialSkill ns = new tPDSSpecialSkill();
                            ns.controlNo = tmpCode;
                            ns.EIC = id;
                            ns.tag = 3;
                            ns.type = "RECOG";
                            ns.pageNo = 1;
                            db.tPDSSpecialSkills.Add(ns);
                            db.SaveChanges();
                        }
                        //DELETE EXTRA DECOY
                    }
                    else
                    {
                        int recogPageCounter = recog.Where(e => e.tag == 1).Count();
                        if (recogPageCounter != 6)
                        {
                            //RE PAGING SETUP
                            int tmpCounter = 0;
                            int tmpPageCounter = 1;
                            foreach (tPDSSpecialSkill item in recog)
                            {
                                tPDSSpecialSkill myData = db.tPDSSpecialSkills.Single(e => e.controlNo == item.controlNo);
                                myData.pageNo = tmpPageCounter;
                                db.SaveChanges();
                                tmpCounter = tmpCounter + 1;
                                if (tmpCounter >= 6)
                                {
                                    tmpCounter = 0;
                                    tmpPageCounter = tmpPageCounter + 1;
                                }
                            }
                        }
                        //DELETE DECOY
                        int recogDel = recognition.Where(e => e.tag > 1).Count();
                        if (recogDel >= 1)
                        {
                            //DELETE DECOY SKILLS
                            IEnumerable<tPDSSpecialSkill> skillDel = otherInfoList.Where(e => e.type == "RECOG" && e.tag > 1).ToList();
                            foreach (tPDSSpecialSkill delItem in skillDel)
                            {
                                tPDSSpecialSkill d = db.tPDSSpecialSkills.Single(e => e.controlNo == delItem.controlNo);
                                db.tPDSSpecialSkills.Remove(d);
                                db.SaveChanges();
                            }
                        }
                    }


                    // ORG. MEMBERSHIP ************************** 
                    IEnumerable<tPDSSpecialSkill> membershipList = otherInfoList.Where(e => e.type == "MEMBER").ToList();
                    IEnumerable<tPDSSpecialSkill> membership = membershipList.Where(e => e.tag == 1).ToList();
                    int memCount = membership.Count();
                    if (memCount <= 6)
                    {
                        int memDecoy = membershipList.Where(e => e.tag > 1).Count();
                        int loopCount = 6 - (memCount + memDecoy);
                        for (int counter = 0; loopCount > counter; counter++)
                        {
                            string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "ORGM" + dt.ToString("ssfff") + counter.ToString("00");
                            tPDSSpecialSkill ns = new tPDSSpecialSkill();
                            ns.controlNo = tmpCode;
                            ns.EIC = id;
                            ns.tag = 3;
                            ns.type = "MEMBER";
                            ns.pageNo = 1;
                            db.tPDSSpecialSkills.Add(ns);
                            db.SaveChanges();
                        }
                    }
                    else
                    {

                    }


                }
                else if (code == 4)
                {
                    reportPage = "PDSPAGE4";
                }

                if (reportPage == "")
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }


                Session["ReportType"] = reportPage;
                Session["PrintReport"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

        }


        //end of code block

    }
}