using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Globalization;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]
    [Authorize(Roles = "APRD, PBOStaff")]

    public class EmployeeController : Controller
    {

        HRISDBEntities db = new HRISDBEntities();

        // GET: Employee
        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveEmployeeRegistration(tRSPEmployee data)
        {
            try
            {

                string userId = Session["_EIC"].ToString();

                string msg = HasEmployeeRegistrationError(data);

                if (msg != "success")
                {
                    return Json(new { status = msg }, JsonRequestBehavior.AllowGet);
                }
                string tmpMidName = "";
                if (data.middleName != null)
                {
                    if (data.middleName.Length > 0)
                    {
                        tmpMidName = data.middleName.ToUpper().Trim();
                    }
                }

                string tmpExtn = "";
                if (data.extName != null)
                {
                    if (data.extName.Length > 0)
                    {
                        tmpExtn = data.extName.ToUpper().Trim();
                    }
                }


                //CHECK ID
                if (data.idNo == null || data.idNo.Length != 4)
                {
                    return Json(new { status = "Invalid ID number!" }, JsonRequestBehavior.AllowGet);
                }
                
                DateTime dob = DateTime.Parse(data.birthDate.ToString(), CultureInfo.CreateSpecificCulture("en-US"));
                
                string tempEIC = GenerateEIC(data);

                tRSPEmployee emp = new tRSPEmployee();
                emp.EIC = tempEIC;
                emp.idNo = data.idNo;
                emp.lastName = data.lastName.ToUpper().Trim();
                emp.firstName = data.firstName.ToUpper().Trim();
                emp.middleName = tmpMidName;

                string tmpFullNameLast = "";
                string tmpFullNameFirst = "";
                string name2 = data.firstName.ToUpper().Trim();

                if (tmpExtn.Length > 0)
                {
                    name2 = name2 + " " + data.extName.ToUpper().Trim();
                }

                tmpFullNameFirst = data.firstName;
                tmpFullNameLast = data.lastName.ToUpper().Trim() + ", " + name2;

                if (tmpMidName.Length > 0)
                {
                    tmpFullNameFirst = tmpFullNameFirst + " " + tmpMidName.Substring(0, 1).ToUpper() + ".";
                    tmpFullNameLast = tmpFullNameLast + " " + tmpMidName.Substring(0, 1).ToUpper() + ".";
                }

                tmpFullNameFirst = tmpFullNameFirst + " " + data.lastName.ToUpper().Trim();

                if (tmpExtn.Length > 0)
                {
                    tmpFullNameFirst = tmpFullNameFirst + " " + tmpExtn;
                }

                emp.fullNameFirst = tmpFullNameFirst.Trim();
                emp.fullNameLast = tmpFullNameLast.Trim();
                emp.extName = tmpExtn;
                emp.sex = data.sex.ToUpper();
                emp.birthDate = dob;
                emp.tag = 1;
                db.tRSPEmployees.Add(emp);

                tRSPTransactionLog log = new tRSPTransactionLog();
                log.logCode = tempEIC;
                log.logType = "REGISTRATION";
                log.logDetails = tmpFullNameFirst.Trim();
                log.userEIC = userId;
                log.logDT = DateTime.Now;
                db.tRSPTransactionLogs.Add(log);
                
                //COMIT
                db.SaveChanges();

                HRISEntities dbHRIS = new HRISEntities();
                tappEmployee app = new tappEmployee();
                app.EIC = tempEIC;
                app.lastName = data.lastName;
                app.firstName = data.firstName;

                if (data.middleName != null)
                {
                    app.middleName = tmpMidName;
                }
               
                app.fullnameFirst = tmpFullNameFirst;
                app.fullnameLast = tmpFullNameLast;
                app.extName = tmpExtn;
                app.birthdate = data.birthDate;
                app.gender = data.sex == "MALE" ? 1 : 2;
                app.sex = data.sex;

                dbHRIS.tappEmployees.Add(app);
                dbHRIS.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private string HasEmployeeRegistrationError(tRSPEmployee data)
        {
            string res = "success";

            if (data.lastName == null)
            {
                return "Please fill-up the required field!";
            }

            if (data.lastName == "" || data.lastName.Length < 2)
            {
                return "Please fill-up the required field!";
            }

            if (data.firstName == null)
            {
                return "Please fill-up the required field!";
            }

            if (data.firstName == "" || data.firstName.Length < 2)
            {
                return "Please fill-up the required field!";
            }

            if (data.birthDate == null)
            {
                return "Invalid birth date!";
            }

            if (data.birthDate.Value.Year < 1000)
            {
                return "Invalid birth date!";
            }
            if (data.birthDate.Value.Year >= DateTime.Now.Year)
            {
                return "Invalid birth date!";
            }

            if (data.sex != "MALE" && data.sex != "FEMALE")
            {
                return "Please fill-up the required field!";
            }

            //check if eXIST
            int count = db.tRSPEmployees.Where(e => e.lastName == data.lastName && e.firstName == data.firstName && e.birthDate == data.birthDate).Count();

            if (count >= 1)
            {
                return "Record already exist!";
            }

            return res;
        }


        [HttpPost]
        public JsonResult EmployeeList()
        {
            try
            {
                var list = db.vRSPEmployeeLists.Select(e => new
                {
                    e.EIC,
                    e.idNo,
                    e.fullNameLast,
                    e.fullNameFirst,
                    e.sex,
                    e.positionTitle,
                    e.subPositionTitle,
                    e.salaryGrade,
                    e.step,
                    e.salaryRate,
                    e.salaryType,
                    e.isGovService,
                    e.employmentStatusTag,
                    e.workGroupName,
                    e.civilStatCode,
                    e.recNo,
                    e.tag
                }).Where(e => e.isGovService == 1).OrderBy(o => o.fullNameLast).ToList();
                return Json(new { status = "success", empList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EmployeeSearch(string id)
        {
            try
            {
                var list = db.vRSPEmployeeLists.Select(e => new
                {
                    e.EIC,
                    e.idNo,
                    e.fullNameLast,
                    e.fullNameFirst,
                    e.sex,
                    e.positionTitle,
                    e.subPositionTitle,
                    e.salaryGrade,
                    e.step,
                    e.salaryRate,
                    e.salaryType,
                    e.isGovService,
                    e.employmentStatusTag,
                    e.workGroupName,
                    e.recNo,
                    e.tag
                }).Where(e => e.fullNameLast.Contains(id)).OrderBy(o => o.fullNameLast).ToList();
                return Json(new { status = "success", empList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult ProfileData(string id)
        {
            try
            {
                vRSPEmployeeList emp = db.vRSPEmployeeLists.Single(e => e.EIC == id);
                Session["ProfileID"] = emp.idNo;
                Session["CurSelEIC"] = emp.EIC;

                string uID = Convert.ToInt16(emp.idNo).ToString("0000");
                string path = @"C:\DataFile\images\" + uID + ".jpg";
                if (System.IO.File.Exists(path))
                {
                    byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                    string imageBase64Data = Convert.ToBase64String(imageByteData);
                    string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                    return Json(new { status = "success", profileData = emp, img = imageDataURL }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    path = @"C:\DataFile\images\0000.jpg";
                    byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                    string imageBase64Data = Convert.ToBase64String(imageByteData);
                    string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                    return Json(new { status = "success", profileData = emp, img = imageDataURL }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetEmployeeImage()
        {
            string id = Session["ProfileID"].ToString();
            string uID = Convert.ToInt16(id).ToString("0000");

            string path = @"C:\DataFile\images\" + uID + ".jpg";

            if (System.IO.File.Exists(path))
            {
                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                //Session["UImage"] = imageDataURL;
                return Json(new { status = "success", img = imageDataURL }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                path = @"C:\DataFile\images\0000.jpg";
                byte[] imageByteData = System.IO.File.ReadAllBytes(path);
                string imageBase64Data = Convert.ToBase64String(imageByteData);
                string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
                //Session["UImage"] = imageDataURL;
                return Json(new { status = "success", img = imageDataURL }, JsonRequestBehavior.AllowGet);
            }


        }

        //[HttpPost]
        //public JsonResult EmployeeNonPlantillaList()
        //{
        //    try
        //    {
        //        var list = db.vRSPEmployees.Select(e => new
        //        {
        //            e.EIC,
        //            e.idNo,
        //            e.fullNameLast,
        //            e.fullNameFirst,
        //            e.sex,
        //            e.positionTitle,
        //            e.salaryGrade,
        //            e.step,
        //            e.workGroupName,
        //            e.tag,
        //            e.employmentStatusCode
        //        }).Where(e => e.tag != 0 && e.employmentStatusCode == null).OrderBy(e => e.fullNameLast).ToList();

        //        return Json(new { status = "success", empList = list }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        public class HRTempData
        {
            public string idNo { get; set; }
            public string eic { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string middlename { get; set; }
            public string extName { get; set; }
            public DateTime? birthDate { get; set; }
            public string birthPlace { get; set; }
            public string fullnameFirst { get; set; }
            public string fullnameLast { get; set; }
            public string sex { get; set; }
        }

        public JsonResult SearchHRISTempData(string id)
        {
            try
            {
                List<HRTempData> list = new List<HRTempData>();

                string lnk = "http://apiserver:8067/api/HRMDATA/Search?id=" + id;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(lnk);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new
                StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        //id = id
                    });
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    dynamic tempStuff = JsonConvert.DeserializeObject(result);
                    dynamic stuff = tempStuff.list;

                    foreach (var item in stuff)
                    {
                        if (item.birthDate == null)
                        {
                            list.Add(new HRTempData()
                            {
                                eic = item.eic,
                                idNo = item.idNo,
                                lastName = item.lastName,
                                firstName = item.firstName,
                                middlename = item.middlename == null ? "" : item.middlename,
                                extName = item.extName,
                                birthDate = null,
                                fullnameLast = item.fullnameLast,
                                fullnameFirst = item.fullnameFirst,
                                sex = item.sex
                            });
                        }
                        else
                        {
                            list.Add(new HRTempData()
                            {
                                eic = item.eic,
                                idNo = item.idNo,
                                lastName = item.lastName,
                                firstName = item.firstName,
                                middlename = item.middlename == null ? "" : item.middlename,
                                extName = item.extName,
                                birthDate = Convert.ToDateTime(item.birthDate),
                                fullnameLast = item.fullnameLast,
                                fullnameFirst = item.fullnameFirst,
                                sex = item.sex
                            });
                        }
                    }
                }
                Session["HRTempData"] = list;
                return Json(new { status = "success", searchList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult MigrateEmployeeDataByID(string id)
        {
            try
            {

                int stat = 0;

                tRSPEmployee employee = db.tRSPEmployees.SingleOrDefault(e => e.EIC == id);
                if (employee == null)
                {
                    var tmpList = Session["HRTempData"];
                    IEnumerable<HRTempData> hrData = tmpList as IEnumerable<HRTempData>;

                    HRTempData tempData = hrData.Single(e => e.eic == id);

                    tRSPEmployee emp = new tRSPEmployee();
                    string tmp = "";
                    if (tempData.extName != null)
                    {
                        tmp = tempData.extName;
                    }

                    if (tempData.middlename != null || tempData.middlename != "")
                    {
                        emp.middleName = tempData.middlename;
                    }

                    emp.EIC = tempData.eic;
                    emp.idNo = tempData.idNo;
                    emp.lastName = tempData.lastName.Trim();
                    emp.firstName = tempData.firstName.Trim();

                    emp.extName = tmp;
                    emp.birthDate = Convert.ToDateTime(tempData.birthDate);
                    emp.fullNameLast = tempData.fullnameLast;
                    emp.fullNameFirst = tempData.fullnameFirst;
                    emp.sex = tempData.sex;
                    db.tRSPEmployees.Add(emp);
                    db.SaveChanges();
                    stat = 1;

                }
                return Json(new { status = "success", stat = stat }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Profile()
        {
            return View();
        }


        public JsonResult ProfileUpdate(tRSPEmployee data)
        {
            try
            {
                string empEIC = data.EIC;



                string fullNameLast = data.lastName + ", " + data.firstName;
                string fullNameFirst = data.firstName;
                string fullNameTitle = "";

                if (data.middleName != null && data.middleName != "")
                {
                    fullNameLast = fullNameLast + " " + data.middleName.Substring(0, 1) + ".";
                    fullNameFirst = fullNameFirst + " " + data.middleName.Substring(0, 1) + ".";
                }
                fullNameFirst = fullNameFirst + " " + data.lastName;
                if (data.extName != null && data.extName != "")
                {
                    fullNameLast = fullNameLast + " " + data.extName;
                    fullNameFirst = fullNameFirst + " " + data.extName;
                }

                string extName = data.extName;
                if (data.extName == null)
                {
                    extName = "";
                }
                if (data.middleName == null)
                {
                    data.middleName = "";
                }

                fullNameTitle = fullNameFirst;
                if (data.nameSuffix != null && data.nameSuffix != "")
                {
                    fullNameTitle = fullNameFirst.Trim() + ", " + data.nameSuffix;
                }

                tRSPEmployee emp = db.tRSPEmployees.SingleOrDefault(e => e.EIC == empEIC);

                if (emp != null)
                {
                    emp.idNo = data.idNo;
                    emp.lastName = data.lastName;
                    emp.firstName = data.firstName;
                    emp.middleName = data.middleName;
                    emp.extName = extName;

                    emp.namePrefix = data.namePrefix;
                    emp.nameSuffix = data.nameSuffix;

                    emp.fullNameLast = fullNameLast;
                    emp.fullNameFirst = fullNameFirst;
                    emp.fullNameTitle = fullNameTitle;

                    if (data.birthDate != null)
                    {
                        DateTime dob = DateTime.Parse(data.birthDate.ToString(), CultureInfo.CreateSpecificCulture("en-US"));
                        emp.birthDate = dob;
                    }

                    emp.birthPlace = data.birthPlace;
                    emp.sex = data.sex;
                    emp.civilStatCode = data.civilStatCode;
                    emp.height = data.height;
                    emp.weight = data.weight;
                    emp.bloodType = data.bloodType;
                    emp.GSISIDNo = data.GSISIDNo;
                    emp.BPNo = data.BPNo;
                    emp.HDMFNo = data.HDMFNo;
                    emp.PHICNo = data.PHICNo;
                    emp.SSSNo = data.SSSNo;
                    emp.TINNo = data.TINNo;

                    emp.citizenship = data.citizenship;
                    emp.houseBlockNoResAddress = data.houseBlockNoResAddress;
                    emp.streetResAddress = data.streetResAddress;
                    emp.subdivisionResAddress = data.subdivisionResAddress;
                    emp.brgyResAddress = data.brgyResAddress;
                    emp.cityMunResAddress = data.cityMunResAddress;
                    emp.provinceResAddress = data.provinceResAddress;
                    emp.ZIPResAddress = data.ZIPResAddress;

                    emp.houseBlockNoPermAddress = data.houseBlockNoPermAddress;
                    emp.streetPermAddress = data.streetPermAddress;
                    emp.subdivisionPermAddress = data.subdivisionPermAddress;
                    emp.brgyPermAddress = data.brgyPermAddress;
                    emp.cityMunPermAddress = data.cityMunPermAddress;
                    emp.provincePermAddress = data.provincePermAddress;
                    emp.ZIPPermAddress = data.ZIPPermAddress;

                    emp.telephoneNo = data.telephoneNo;
                    emp.mobileNo = data.mobileNo;
                    emp.emailAddress = data.emailAddress;
                    db.SaveChanges();
                    
                    HRISEntities dbHRIS = new HRISEntities();
                    tappEmployee app = dbHRIS.tappEmployees.SingleOrDefault(e => e.EIC == empEIC);

                    if (app != null)
                    {
                        if (app.idNo == null)
                        {
                            app.idNo = data.idNo;
                            dbHRIS.SaveChanges();
                        }
                    }

                }

                 

                return Json(new { status = "success", data = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateFamilyBackGround(tPDSProfile data)
        {
            tPDSProfile p = db.tPDSProfiles.Single(e => e.EIC == data.EIC);
            p.spouseLastName = data.spouseLastName;
            p.spouseFirstName = data.spouseFirstName;
            p.spouseMiddleName = data.spouseMiddleName;
            p.spouseExtnName = data.spouseExtnName;
            p.spouseOccupation = data.spouseOccupation;
            p.spouseEmployeerName = data.spouseEmployeerName;
            p.spouseEmployeerAddrs = data.spouseEmployeerAddrs;
            p.spouseEmployeerTelNo = data.spouseEmployeerTelNo;
            //father
            p.fatherLastName = data.fatherLastName;
            p.fatherFirstName = data.fatherFirstName;
            p.fatherMidddleName = data.fatherMidddleName;
            p.fatherExtnName = data.fatherExtnName;
            //mother 
            p.motherLastName = data.motherLastName;
            p.motherFirstName = data.motherFirstName;
            p.motherMiddleName = data.motherMiddleName;
            db.SaveChanges();
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetPDSEducation(string id)
        {
            IEnumerable<tPDSEducation> educationList = db.tPDSEducations.Where(e => e.EIC == id).OrderBy(o => o.educLevelCode).ThenBy(o => o.periodYearFrom).ToList();


            var educ = (from e in db.tPDSEducations
                        join l in db.tPDSEducationLevels on e.educLevelCode equals l.educLevelCode
                        where e.EIC == id
                        select new
                        {
                            e.controlNo,
                            l.educLevelCode,
                            l.educLevelName,
                            e.schoolName,
                            e.degree,
                            e.periodYearFrom,
                            e.periodYearTo,
                            e.highestLevel,
                            e.yearGraduated,
                            e.honorsReceived,
                            e.isVerified
                        }).OrderBy(o => o.educLevelCode).ThenBy(o => o.periodYearFrom).ToList();


            return Json(new { status = "success", educationList = educ }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConfirmPDSEducation(tPDSEducation data, string reason)
        {

            tPDSEducation item = db.tPDSEducations.SingleOrDefault(e => e.controlNo == data.controlNo);

            if (item == null)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

            string uEIC = Session["_EIC"].ToString();

            item.remarks = "";
            if (data.isVerified == 0)
            {
                item.isVerified = 0;
                item.remarks = reason;
            }
            item.verifiedByEIC = uEIC;
            item.verifiedDT = DateTime.Now;
            db.SaveChanges();

            string id = item.EIC;
            IEnumerable<tPDSEducation> educationList = db.tPDSEducations.Where(e => e.EIC == id).OrderBy(o => o.educLevelCode).ThenBy(o => o.periodYearFrom).ToList();
            var educ = (from e in db.tPDSEducations
                        join l in db.tPDSEducationLevels on e.educLevelCode equals l.educLevelCode
                        where e.EIC == id
                        select new
                        {
                            e.controlNo,
                            l.educLevelCode,
                            l.educLevelName,
                            e.schoolName,
                            e.degree,
                            e.periodYearFrom,
                            e.periodYearTo,
                            e.highestLevel,
                            e.yearGraduated,
                            e.honorsReceived,
                            e.isVerified
                        }).OrderBy(o => o.educLevelCode).ThenBy(o => o.periodYearFrom).ToList();


            return Json(new { status = "success", educationList = educ }, JsonRequestBehavior.AllowGet);



        }


        [HttpPost]
        public JsonResult GetFamilyData(string id)
        {

            tPDSProfile famData = new tPDSProfile();
            tPDSProfile fam = db.tPDSProfiles.SingleOrDefault(e => e.EIC == id);
            if (fam != null)
            {
                famData = fam;
            }
            else
            {
                tPDSProfile p = new tPDSProfile();
                p.EIC = id;
                db.tPDSProfiles.Add(p);
                db.SaveChanges();
            }
            IEnumerable<tPDSChildren> child = db.tPDSChildrens.Where(e => e.EIC == id && e.birthDate != null).OrderBy(o => o.birthDate).ToList();
            return Json(new { status = "success", famData = famData, children = child }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SubmitChildData(tPDSChildren data)
        {
            tPDSChildren c = new tPDSChildren();
            c.EIC = data.EIC;
            c.childName = data.childName.ToUpper().Trim();
            c.birthDate = data.birthDate;
            db.tPDSChildrens.Add(c);
            db.SaveChanges();
            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ServiceRecord()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ServiceRecordData(string id)
        {
            try
            {

                if (id == null || id == "")
                {
                    id = Session["CurSelEIC"].ToString();
                }
                else
                {
                    Session["CurSelEIC"] = id;
                }

                tRSPEmployee employee = db.tRSPEmployees.Single(e => e.EIC == id);
                ServiceRecord sr = new ServiceRecord();

                sr.EIC = employee.EIC;
                sr.idNo = employee.idNo;
                sr.lastName = employee.lastName;
                sr.firstName = employee.firstName;
                sr.middleName = employee.middleName;
                sr.birthDate = employee.birthDate;
                sr.birthPlace = employee.birthPlace;
                sr.extnName = employee.extName;

                List<tRSPServiceRecord> record = GetServiceRecordData(id);
                sr.serviceData = record;

                int tag = 0;
                if (record.Count() > 0)
                { tag = 1; }

                //SERVER RECORD AWOL; GAPS
                string srRemarks = "";
                IEnumerable<tRSPServiceRecordRemark> rem = db.tRSPServiceRecordRemarks.Where(e => e.EIC == employee.EIC).ToList();

                if (rem.Count() > 0)
                {
                    int counter = 0;
                    foreach (tRSPServiceRecordRemark item in rem)
                    {
                        counter = counter + 1;
                        srRemarks = srRemarks + item.remarks;
                        if (counter < rem.Count())
                        {
                            srRemarks = srRemarks + "; ";
                        }
                    }
                    srRemarks = srRemarks.Trim();
                }

                tRSPServiceRecordLog log = db.tRSPServiceRecordLogs.OrderBy(o => o.transDT).SingleOrDefault(e => e.EIC == id && e.tag == 1);
                string logStr = "";
                if (log != null)
                {
                    logStr = log.remarks; srRemarks = srRemarks + "; " + logStr;
                }

                sr.lastRemarks = logStr;
                sr.GAPRemarks = srRemarks;

                return Json(new { status = "success", serviceRecord = sr, tag = tag }, JsonRequestBehavior.AllowGet);

                //CHECK IF ALREADY MIGRATED
                //int tag = 0;
                //IEnumerable<tRSPServiceRecord> srvRecord = db.tRSPServiceRecords.Where(e => e.EIC == employee.EIC).OrderBy(o => o.dateFrom).ToList();

                //if (srvRecord.Count() > 0)
                //{ tag = 1; }
                //sr.EIC = employee.EIC;
                //sr.idNo = employee.idNo;
                //sr.lastName = employee.lastName;
                //sr.firstName = employee.firstName;
                //sr.middleName = employee.middleName;
                //sr.birthPlace = employee.birthPlace;
                //sr.extnName = employee.extName;
                //List<ServiceData> record = new List<ServiceData>();
                //List<tRSPServiceRecord> record = new List<tRSPServiceRecord>();
                //foreach (var item in srvRecord)
                //{
                //    record.Add(new tRSPServiceRecord()
                //    {
                //        recNo = item.recNo,
                //        dateFrom = Convert.ToDateTime(item.dateFrom),
                //        dateTo = item.dateTo,
                //        dateToText = item.dateToText,
                //        positionTitle = item.positionTitle,
                //        employmentStatus = item.employmentStatus,
                //        salary = Convert.ToDecimal(item.salary),
                //        salaryType = item.salaryType,
                //        placeOfAssignment = item.placeOfAssignment,
                //        branch = item.branch,
                //        LWOP = item.LWOP,
                //        separationCause = item.separationCause
                //    });
                //}
                //sr.serviceData = record;



                //sr.lastRemarks = logStr;
                //sr.GAPRemarks = srRemarks;
                //tag = 1;



            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ChecMySQLServiceRecord(string id)
        {

            //id = Session["CurSelEIC"].ToString();
            if (id == null || id == "")
            {
                id = Session["CurSelEIC"].ToString();
            }

            try
            {
                tRSPEmployee employee = db.tRSPEmployees.Single(e => e.EIC == id);
                string tmpID = Convert.ToInt16(employee.idNo).ToString("0000");
                ServiceRecord sr = new ServiceRecord();
                string lnk = "http://apiserver:8067/api/service/getdata?id=" + tmpID;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(lnk);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new
                StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        //id = id
                    });
                    streamWriter.Write(json);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {

                    string result = streamReader.ReadToEnd();
                    dynamic tempStuff = JsonConvert.DeserializeObject(result);
                    dynamic emp = tempStuff.emp;
                    dynamic records = tempStuff.records;

                    sr.idNo = emp.iD_no;
                    sr.lastName = emp.lname;
                    sr.firstName = emp.fname;
                    sr.middleName = emp.fmi;
                    sr.birthPlace = emp.bplace;
                    sr.extnName = emp.ext;
                    sr.birthDate = Convert.ToDateTime(emp.bday);
                    sr.remarks = emp.remarks;
                    sr.remarks2 = emp.remarks2;
                    sr.remarks3 = emp.remarks3;
                    sr.lastRemarks = emp.lastRemarks;

                    List<tRSPServiceRecord> record = new List<tRSPServiceRecord>();

                    foreach (var item in records)
                    {
                        record.Add(new tRSPServiceRecord()
                        {
                            dateFrom = item.dfrom,
                            dateToText = item.dto,
                            positionTitle = item.designation,
                            employmentStatus = item.empStat,
                            salary = item.salary,
                            salaryType = item.salType,
                            placeOfAssignment = item.office,
                            branch = item.branch,
                            LWOP = item.lwpay,
                            separationCause = item.sepcause1
                        });
                    }

                    record = record.OrderBy(o => o.dateFrom).ToList();
                    sr.serviceData = record;
                    Session["ServiceEmp"] = emp;
                    Session["ServiceRecord"] = sr;
                }
                return Json(new { status = "success", serviceRecord = sr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        private List<tRSPServiceRecord> GetServiceRecordData(string id)
        {
            List<tRSPServiceRecord> record = new List<tRSPServiceRecord>();
            IEnumerable<tRSPServiceRecord> list = db.tRSPServiceRecords.Where(e => e.EIC == id).OrderBy(o => o.dateFrom).ToList();
            foreach (tRSPServiceRecord item in list)
            {
                record.Add(new tRSPServiceRecord()
                {
                    //recNo = item.recNo,
                    transCode = item.transCode,
                    dateFrom = item.dateFrom,
                    dateTo = item.dateTo,
                    dateToText = item.dateToText,
                    positionTitle = item.positionTitle,
                    employmentStatus = item.employmentStatus,
                    salary = item.salary,
                    salaryType = item.salaryType,
                    placeOfAssignment = item.placeOfAssignment,
                    branch = item.branch,
                    LWOP = item.LWOP,
                    separationCause = item.separationCause,
                    isSysGen = item.isSysGen
                });
            }
            return record;
        }

        [HttpPost]
        public JsonResult MigrateServiceRecord()
        {
            try
            {
                var temp = Session["ServiceRecord"];
                string uEIC = Session["_EIC"].ToString();
                ServiceRecord serviceRecord = temp as ServiceRecord;
                string tmpID = Session["CurSelEIC"].ToString();
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == tmpID);
                DateTime dt = DateTime.Now;

                string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "SRM" + dt.ToString("ssfff") + tmpID.Substring(0, 3); //19
                int counter = 0;
                foreach (tRSPServiceRecord item in serviceRecord.serviceData)
                {
                    tRSPServiceRecord s = new tRSPServiceRecord();
                    if (item.dateToText.Length > 8)
                    {
                        string tmp = item.dateToText;
                        if (tmp.Length > 8)
                        {
                            if (tmp == "04/31/1984")
                            {
                                tmp = "04/30/1984";
                            }
                            s.dateTo = Convert.ToDateTime(tmp);
                        }
                    }
                    counter = counter + 1;
                    string code = tmpCode + counter.ToString("000");

                    s.transCode = code;
                    s.EIC = emp.EIC;
                    s.dateFrom = item.dateFrom;
                    s.dateToText = item.dateToText;
                    s.positionTitle = item.positionTitle;
                    s.employmentStatus = item.employmentStatus;
                    s.salary = Convert.ToDecimal(item.salary);
                    s.salaryType = item.salaryType;
                    s.placeOfAssignment = item.placeOfAssignment;
                    s.LWOP = item.LWOP;
                    s.separationCause = item.separationCause;
                    s.branch = item.branch;
                    s.remarks = "";
                    s.loyaltyTag = 1;
                    s.tag = 1;
                    s.isSysGen = 0;
                    s.transDT = DateTime.Now;
                    s.userEIC = uEIC;
                    db.tRSPServiceRecords.Add(s);
                }

                string srRemarks = serviceRecord.remarks;
                if (serviceRecord.remarks2.Length > 5)
                {
                    srRemarks = srRemarks + serviceRecord.remarks2;
                }

                if (serviceRecord.lastRemarks.Length > 5)
                {
                    tRSPServiceRecordLog log = new tRSPServiceRecordLog();
                    log.EIC = emp.EIC;
                    log.remarks = serviceRecord.lastRemarks.Trim();
                    log.transDT = DateTime.Now;
                    log.tag = 1;
                    db.tRSPServiceRecordLogs.Add(log);
                }

                string[] newRemarks = srRemarks.Split(';');
                if (newRemarks.Length >= 1)
                {
                    foreach (string item in newRemarks)
                    {
                        tRSPServiceRecordRemark r = new tRSPServiceRecordRemark();
                        r.EIC = emp.EIC;
                        r.remarks = item.Trim();
                        r.tag = 1;
                        db.tRSPServiceRecordRemarks.Add(r);
                    }
                }

                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string tmp = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //SAVE NEW SERVICE RECORD
        [HttpPost]
        public JsonResult SaveNewRecord(tRSPServiceRecord data, int presentTag)
        {
            try
            {
                string tmpEIC = Session["CurSelEIC"].ToString();
                string uEIC = Session["_EIC"].ToString();

                DateTime dt = DateTime.Now;
                string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "SRE" + dt.ToString("ssfff") + tmpEIC.Substring(0, 6); //19

                tRSPServiceRecord n = new tRSPServiceRecord();
                if (presentTag == 1)
                {
                    n.dateToText = "PRESENT";
                }
                else
                {
                    DateTime tmpDateTo = Convert.ToDateTime(data.dateTo);
                    if (tmpDateTo.Year > 1000)
                    {
                        n.dateTo = tmpDateTo; n.dateToText = tmpDateTo.ToString("MM/dd/yyyy");
                    }
                    else
                    { n.dateToText = " / / "; }
                }

                n.transCode = tmpCode;
                n.EIC = tmpEIC;
                n.dateFrom = data.dateFrom;

                n.positionTitle = data.positionTitle;
                n.employmentStatus = data.employmentStatus;
                n.salary = data.salary;
                n.salaryType = data.salaryType;
                n.placeOfAssignment = data.placeOfAssignment;
                n.branch = data.branch;
                n.LWOP = data.LWOP;
                n.separationCause = data.separationCause;
                n.transDT = dt;
                n.userEIC = uEIC;
                n.tag = 1; //not for update
                n.loyaltyTag = 1; //loyalty
                n.isSysGen = 0; // is system generated record
                db.tRSPServiceRecords.Add(n);
                db.SaveChanges();

                List<tRSPServiceRecord> myList = GetServiceRecordData(tmpEIC);
                return Json(new { status = "success", serviceRecord = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string tmp = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //UDATE SERVICE RECORD
        [HttpPost]
        public JsonResult UdateServiceRecod(tRSPServiceRecord data, int presentTag)
        {
            try
            {
                string id = Session["CurSelEIC"].ToString();
                tRSPServiceRecord s = db.tRSPServiceRecords.Single(e => e.transCode == data.transCode && e.EIC == id);
                if (s != null)
                {
                    if (presentTag == 1)
                    {
                        s.dateToText = "PRESENT";
                    }
                    else
                    {
                        DateTime tmpDateTo = Convert.ToDateTime(data.dateTo);
                        if (tmpDateTo.Year > 1000)
                        {
                            s.dateTo = tmpDateTo; s.dateToText = tmpDateTo.ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            s.dateToText = " / / ";
                        }
                    }
                    s.placeOfAssignment = data.placeOfAssignment;
                    s.branch = data.branch;
                    s.LWOP = data.LWOP;
                    s.separationCause = data.separationCause;
                    db.SaveChanges();

                    List<tRSPServiceRecord> myList = GetServiceRecordData(id);

                    return Json(new { status = "success", serviceRecord = myList }, JsonRequestBehavior.AllowGet);

                }
                return Json(new { status = "Unable to update record!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string tmp = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PrintServiceRecord(string id)
        {
            try
            {
                string eEIC = Session["CurSelEIC"].ToString();
                tRSPEmployee employee = db.tRSPEmployees.Single(e => e.EIC == eEIC);

                string srRemarks = "";
                IEnumerable<tRSPServiceRecordRemark> rem = db.tRSPServiceRecordRemarks.Where(e => e.EIC == employee.EIC).ToList();

                if (rem.Count() > 0)
                {
                    int counter = 0;
                    foreach (tRSPServiceRecordRemark item in rem)
                    {
                        counter = counter + 1;
                        srRemarks = srRemarks + item.remarks;
                        if (counter < rem.Count())
                        {
                            srRemarks = srRemarks + "; ";
                        }
                    }
                    srRemarks = srRemarks.Trim();
                }

                tRSPServiceRecordLog log = db.tRSPServiceRecordLogs.OrderBy(o => o.transDT).SingleOrDefault(e => e.EIC == employee.EIC && e.tag == 1);
                string logStr = "";
                if (log != null)
                {
                    logStr = log.remarks;
                    srRemarks = srRemarks + "; " + logStr;
                }

                Session["ReportType"] = "ServiceRecord";
                Session["PrintReport"] = id + "|" + employee.EIC + "|" + srRemarks + "|" + logStr;

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string tmp = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public JsonResult SetupEmployee(string id)
        {
            try
            {

                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == id);
                Session["CurSelEIC"] = id;
                Session["ProfileID"] = emp.idNo;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult RetrieveImage()
        {

            //string paths = @"C:\DataFile\images\0000.jpg";
            //return File(paths, "image/png");

            try
            {
                string tmpID = Session["ProfileID"].ToString();
                string uID = Convert.ToInt16(tmpID).ToString("0000");
                string path = @"C:\DataFile\images\" + uID + ".jpg";

                if (System.IO.File.Exists(path))
                {
                    return File(path, "image/png");
                }
                else
                {
                    path = @"C:\DataFile\images\0000.jpg";
                    return File(path, "image/png");
                }

            }
            catch
            {
                string path = @"C:\DataFile\images\0000.jpg";
                return File(path, "image/png");
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////
        // SERVICE DATES
        public class ServiceDateData
        {
            public string EIC { get; set; }
            public DateTime dateOrigAppointment { get; set; }
            public DateTime dateLastPromoted { get; set; }
        }

        [HttpPost]
        public JsonResult GetEmployeeServiceDate(string id)
        {
            try
            {
                tRSPEmployee emp = db.tRSPEmployees.SingleOrDefault(e => e.EIC == id);
                if (emp != null)
                {
                    ServiceDateData sd = new ServiceDateData();
                    sd.dateOrigAppointment = Convert.ToDateTime(emp.dateOrigAppointment);
                    sd.dateLastPromoted = Convert.ToDateTime(emp.dateLastPromoted);
                    return Json(new { status = "success", serviceDate = sd }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult UpdateEmployeeServiceDate(ServiceDateData data)
        {
            try
            {

                tRSPEmployee emp = db.tRSPEmployees.SingleOrDefault(e => e.EIC == data.EIC);
                if (emp != null)
                {

                    string userId = Session["_EIC"].ToString();
                    DateTime dt = DateTime.Now;

                    //UPDATE
                    emp.dateOrigAppointment = Convert.ToDateTime(data.dateOrigAppointment);

                    //TRANS LOG
                    string tmpCode = "T" + dt.ToString("yyMMddHHmm") + "UPD" + dt.ToString("ssfff") + userId.Substring(0, 4);
                    tRSPTransactionLog log = new tRSPTransactionLog();
                    log.logCode = tmpCode;
                    log.logType = "SERVICE DATE";
                    log.logDetails = "UPDATE " + emp.fullNameFirst.ToString() + " - " + Convert.ToDateTime(data.dateOrigAppointment).ToString("MM/dd/yyyy");
                    log.userEIC = userId;
                    log.logDT = dt;
                    db.tRSPTransactionLogs.Add(log);

                    db.SaveChanges();

                    return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////
        //WORK GROUP
        ////////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        public JsonResult GetWorkGroupList()
        {
            try
            {
                var wrkGroupList = db.tRSPWorkGroups.Select(e => new
                {
                    e.workGroupCode,
                    e.workGroupName,
                    e.tag,
                    e.orderNo
                }).Where(e => e.tag >= 1).OrderBy(o => o.orderNo).ToList();

                return Json(new { status = "success", workGroupList = wrkGroupList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateWorkGroup(string id, string code, string tag)
        {
            try
            {
                tRSPWorkGroupEmp w = db.tRSPWorkGroupEmps.SingleOrDefault(e => e.EIC == id);
                if (w != null)
                {
                    w.workGroupCode = code;
                    w.userEIC = Session["_EIC"].ToString();
                    w.transDT = DateTime.Now;
                    db.SaveChanges();
                }
                else
                {
                    tRSPWorkGroupEmp n = new tRSPWorkGroupEmp();
                    n.EIC = id;
                    n.workGroupCode = code;
                    n.userEIC = Session["_EIC"].ToString();
                    n.transDT = DateTime.Now;
                    db.tRSPWorkGroupEmps.Add(n);
                    db.SaveChanges();
                }

                if (tag == null || tag == "")
                {
                    var list = db.vRSPEmployeeLists.Select(e => new
                    {
                        e.EIC,
                        e.idNo,
                        e.fullNameLast,
                        e.fullNameFirst,
                        e.sex,
                        e.positionTitle,
                        e.subPositionTitle,
                        e.salaryGrade,
                        e.step,
                        e.isGovService,
                        e.employmentStatusTag,
                        e.workGroupName,
                        e.recNo,
                        e.tag
                    }).Where(e => e.isGovService == 1).OrderBy(o => o.fullNameLast).ToList();
                    return Json(new { status = "success", empList = list }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var list = db.vRSPEmployeeLists.Select(e => new
                    {
                        e.EIC,
                        e.idNo,
                        e.fullNameLast,
                        e.fullNameFirst,
                        e.sex,
                        e.positionTitle,
                        e.subPositionTitle,
                        e.salaryGrade,
                        e.step,
                        e.isGovService,
                        e.employmentStatusTag,
                        e.workGroupName,
                        e.recNo,
                        e.tag
                    }).Where(e => e.fullNameLast.Contains(tag)).OrderBy(o => o.fullNameLast).ToList();
                    return Json(new { status = "success", empList = list }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //ARCHIVE RECORD : ArchiveEmployeeRecord
        public JsonResult ArchiveEmployeeRecord(string id)
        {
            try
            {
                //CHECK IF NOT IN THE PLANTILLA
                //CHECK IF NO CURRENT APPOINTMENT
                // IF YES THEN UPDATE
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == id);
                emp.tag = 0;
                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////
        //201-FILE
        ////////////////////////////////////////////////////////////////////////////////////

        public ActionResult E201File()
        {

            return View();
        }

        [HttpPost]
        public JsonResult Load201Data(string id)
        {
            try
            {
                string uEIC = Session["CurSelEIC"].ToString();

                if (id == null || id == "")
                {
                    id = "APPT";
                }

                IEnumerable<tRSP201File> fileList = db.tRSP201File.Where(e => e.EIC == uEIC && e.docType == id).ToList();

                return Json(new { status = "success", list = fileList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////

        [HttpPost]
        public JsonResult PrintPDS(string id, int code)
        {
            try
            {
                tRSPEmployee emp = db.tRSPEmployees.Single(e => e.EIC == id);
                Session["ReportType"] = "PDSPAGE1";
                Session["PrintReport"] = emp.EIC;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public JsonResult UpdateIDNumber(string id, string code, string value)
        {
            try
            {
                tRSPEmployee emp = db.tRSPEmployees.SingleOrDefault(e => e.EIC == id);
                if (emp != null)
                {
                    string res = "";
                    if (code == "TIN")
                    {
                        if (emp.TINNo == null || emp.TINNo == "")
                        {
                            emp.TINNo = value;
                            res = "success";
                        }
                    }
                    else if (code == "GSISID")
                    {
                        if (emp.GSISIDNo == null || emp.GSISIDNo == "")
                        {
                            emp.GSISIDNo = value;
                            res = "success";
                        }
                    }
                    else if (code == "GSISBP")
                    {
                        if (emp.BPNo == null || emp.BPNo == "")
                        {
                            emp.BPNo = value;
                            res = "success";
                        }
                    }
                    else if (code == "HDMFID")
                    {
                        if (emp.HDMFNo == null || emp.HDMFNo == "")
                        {
                            emp.HDMFNo = value;
                            res = "success";
                        }
                    }
                    else if (code == "HDMFMID")
                    {
                        if (emp.HDMFMID == null || emp.HDMFMID == "")
                        {
                            emp.HDMFMID = value;
                            res = "success";
                        }
                    }
                    else if (code == "PHIC")
                    {
                        if (emp.PHICNo == null || emp.PHICNo == "")
                        {
                            emp.PHICNo = value;
                            res = "success";
                        }
                    }

                    if (res == "success")
                    {
                        db.SaveChanges();
                    }
                    //vRSPEmployeeList emp = db.vRSPEmployeeLists.Single(e => e.EIC == id);
                    return Json(new { status = res, profileData = emp }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public class TempPrintData
        {
            public string fundSourceCode { get; set; }
            public string employmentStatusCode { get; set; }
            public string workGroupCode { get; set; }
            public string printCode { get; set; }

        }

        [HttpPost]
        public JsonResult PrintByEmpList(TempPrintData data)
        {
            if (data.printCode == "CH")
            {
                Session["ReportType"] = "EMPBYCHARGES";
                Session["PrintReport"] = data.fundSourceCode + ":" + data.workGroupCode + ":" + data.employmentStatusCode;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else if (data.printCode == "WB")
            {
                Session["ReportType"] = "EMPOFFICEWORKGROUP";
                Session["PrintReport"] = data.workGroupCode + "|" + data.employmentStatusCode;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        }

        private string GenerateEIC(tRSPEmployee data)
        {
            string hash = "";

            string lastname = data.lastName.Trim().ToUpper();
            lastname = stringTrim(lastname, 1);


            string firstname = data.firstName.Trim().ToUpper();
            firstname = stringTrim(firstname, 1);


            string midIni = data.middleName;
            midIni = stringTrim(midIni, 0);

            // lastname & firstname (1)
            string L = lastname.Substring(0, 1);   //Last L
            string F = firstname.Substring(0, 1);  //first F

            //SECOND SUBSTRING (1)
            string secondL = lastname.Substring(1, 1);   //last 2 & 3
            string secondF = firstname.Substring(1, 1);  //first 2 & 3

            //sex (M/F) (1)
            string SX = data.sex.Substring(0, 1);

            //last letter (1)
            string lastL1 = lastname.Substring(lastname.Length - 1, 1);
            string LastF1 = firstname.Substring(firstname.Length - 1, 1);

            //mid of lastname & firstname 
            int iMidL = (int)lastname.Length / 2;
            int iMIdF = (int)firstname.Length / 2;
            // (1)
            string midL = lastname.Substring(iMidL, 1);
            string midF = firstname.Substring(iMIdF, 1);

            // f & l Length (2)
            int x = Convert.ToInt32(lastname.Length);
            string lenL = x.ToString("00");
            int y = Convert.ToInt32(firstname.Length);
            string lenF = y.ToString("00");
            string dobYY = Convert.ToDateTime(data.birthDate).ToString("yy");
            string dobMM = Convert.ToDateTime(data.birthDate).ToString("MM");
            string dobDD = Convert.ToDateTime(data.birthDate).ToString("dd");
            //string revF3 = RevereseString(F3);
            //string revL3 = RevereseString(L3);
            //string LF3 = lastname.Substring(0, 3);
            //dobYYYY = dobYYYY.Substring(0, 3);
            //dobYYYY = RevereseString(dobYYYY);
            //date DD & MM (4)
            string revDOB = RevereseString(dobDD + dobMM);

            hash = F + L + lenF + lenL + dobYY + SX + secondF + midF + LastF1 + secondL + midL + lastL1 + revDOB + "0";
            return hash;
        }

        private string RevereseString(string myStr)
        {
            char[] myArr = myStr.ToArray();
            Array.Reverse(myArr);
            return new string(myArr);
        }

        private string stringTrim(string str, int tag)
        {
            if (str == null)
            {
                return "";
            }
            str = str.Replace("  ", " ");
            str = str.Replace("  ", " ");
            str = str.Replace(" ", "");
            str = str.Trim();
            if (tag == 1)
            {
                if (str.Length < 3)
                {
                    do
                    {
                        str = str + "0";
                    } while (str.Length < 3);
                }
            }

            return str;
        }

    }
}