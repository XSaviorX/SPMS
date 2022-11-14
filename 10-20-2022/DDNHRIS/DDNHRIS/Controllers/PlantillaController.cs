using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DDNHRIS.Models;

namespace DDNHRIS.Controllers
{

    [SessionTimeout]

    public class PlantillaController : Controller
    {
        HRISDBEntities db = new HRISDBEntities();
        // GET: Plantilla

        [Authorize(Roles = "APRD,PBOStaff")]
        public ActionResult Current()
        {
            return View();
        }

        //FOR COMPETENCY
        // : Competency/ComptAssmntTool.js
        [HttpPost]
        public JsonResult ShowDepartmentPlantillaById(string id)
        {
            try
            {
                IEnumerable<StructureParentViewModel> list = DeptCurrentStructureByOfficeCode(id, true);
                IEnumerable<vLNDCompetencyPositionMap> mapList = db.vLNDCompetencyPositionMaps.Where(e => e.assmntGroupCode == id).ToList(); //HAS TEMP CODE [3100]

                List<StructureParentViewModel> myList = new List<StructureParentViewModel>();

                foreach (StructureParentViewModel item in list)
                {

                    List<PlantillaPosition> rowPosList = new List<PlantillaPosition>();
                    foreach (PlantillaPosition row in item.positionList)
                    {
                        int tag = 0;

                        if (row.itemNo == "0650")
                        {
                            tag = 0;
                        }

                        string comptPosName = "NONE";
                        string comptPositionCode = "";

                        vLNDCompetencyPositionMap myMap = mapList.SingleOrDefault(e => e.plantillaCode == row.plantillaCode);

                        if (myMap != null)
                        {
                            comptPosName = myMap.comptPositionTitle;
                            comptPositionCode = myMap.comptPositionCode;
                            tag = 1;
                        }
                                              
                        row.fullNameLast = comptPosName;
                        row.plantillaNo = tag;
                        row.eligibilityName = comptPositionCode;
                        rowPosList.Add(row);

                    }
                    item.positionList = rowPosList;
                    myList.Add(item);
                }

                return Json(new { status = "success", plantilla = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult ShowCurrentPlantilla(string id)
        {
            try
            {
                //BY FUNCTION CODE
                IEnumerable<StructureParentViewModel> list = DeptCurrentStructure(id, 1);

                //BY OFFICE CODE
                //IEnumerable<StructureParentViewModel> list = DeptCurrentStructureByOfficeCode(id, true);

                Session["PlantillaCurrent"] = list;
                return Json(new { status = "success", plantilla = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ShowCurrentPlantillaVacant(string id)
        {
            try
            {
                //BY FUNCTION CODE
                IEnumerable<StructureParentViewModel> list = DeptCurrentStructure(id, 0);
                Session["PlantillaCurrent"] = list;
                return Json(new { status = "success", plantilla = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

       




        [HttpPost]
        public JsonResult Current(string id, string printType)
        {
            string printCode = "PRNCODE" + DateTime.Now.ToString("yyMMddHHmmhhssfff");

            var tempList = Session["PlantillaCurrent"];
            IEnumerable<StructureParentViewModel> list = tempList as IEnumerable<StructureParentViewModel>;

            tOrgFunction dept = db.tOrgFunctions.Single(e => e.functionCode == id);


            int myYear = DateTime.Now.Year + 1;

            IEnumerable<PBOReportViewModel> ps = PersonnelServices(dept.departmentCode, myYear);

            IEnumerable<vRSPSalaryDetailCurrent> salaryTable = db.vRSPSalaryDetailCurrents.ToList();


            int iCount = 0;
            List<vOrgStructure> tempStructChecker = new List<vOrgStructure>();

            foreach (StructureParentViewModel item in list)
            {
                foreach (PlantillaPosition itm in item.positionList)
                {

                    decimal monthRate = Convert.ToDecimal(salaryTable.Single(e => e.salaryGrade == itm.salaryGrade).rateMonth);

                    iCount = iCount + 1;
                    tRSPZPlantillaCurrent rep = new tRSPZPlantillaCurrent();

                    string clusterName = "";

                    decimal tmpAnnualAuthorize = itm.currentYearRate * 12;
                    decimal tmpAnnualActual = itm.currentYearRate * 12;
                    int tmpStep = 1;

                    if (itm.EIC != null)
                    {
                        PBOReportViewModel tmpPS = ps.Single(e => e.EIC == itm.EIC);
                        tmpAnnualAuthorize = itm.currentYearRate * 12;
                        tmpAnnualActual = tmpPS.annualSalary;
                        tmpStep = tmpPS.stepInc;
                    }


                    if (tmpAnnualAuthorize == 0)
                    {
                        tmpAnnualAuthorize = 0;
                    }

                    int isStructClusExist = tempStructChecker.Where(e => e.structureCode == itm.clusterCode).Count();
                    if (isStructClusExist >= 1)
                    {
                        clusterName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.clusterCode
                        });
                        clusterName = itm.clusterName != null ? itm.clusterName.ToUpper() : itm.clusterName;
                    }


                    string divName = "";
                    int isStructExist = tempStructChecker.Where(e => e.structureCode == itm.divisionCode).Count();
                    if (isStructExist >= 1)
                    {
                        divName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.divisionCode
                        });
                        divName = itm.divisionName;
                    }

                    string sectionName = "";
                    int isStructExistSec = tempStructChecker.Where(e => e.structureCode == itm.sectionCode).Count();
                    if (isStructExistSec >= 1)
                    {
                        sectionName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.sectionCode
                        });
                        sectionName = itm.sectionName;
                    }

                    string unitName = "";
                    int isStructExistUnit = tempStructChecker.Where(e => e.structureCode == itm.unitCode).Count();
                    if (isStructExistUnit >= 1)
                    {
                        unitName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.unitCode
                        });
                        unitName = itm.unitName;
                    }

                    string tmpLevel = "";
                    if (itm.positionLevel == 1)
                    {
                        tmpLevel = "1st";
                    }
                    else if (itm.positionLevel == 2)
                    {
                        tmpLevel = "2nd";
                    }

                    rep.functionCode = id;
                    rep.plantillaCode = itm.plantillaCode;
                    //rep.budgetYear = 2021;
                    rep.oldItemNo = Convert.ToInt16(itm.oldItemNo).ToString("0000");
                    rep.itemNo = Convert.ToInt16(itm.itemNo).ToString("0000");
                    rep.salary = itm.currentYearRate;

                    rep.annualAuthorize = tmpAnnualAuthorize;
                    rep.annualActual = tmpAnnualActual;
                    rep.salary = monthRate;
                    rep.positionTitle = itm.positionTitle;
                    rep.subPositionTitle = itm.subPositionTitle;
                    rep.salaryGrade = itm.salaryGrade;
                    rep.step = tmpStep;

                    rep.positionLevel = tmpLevel;
                    rep.lastName = itm.lastName;
                    rep.firstName = itm.firstName;
                    rep.middleName = itm.middleName;
                    rep.extname = itm.extName;

                    rep.birthDate = itm.birthDate;
                    rep.eligibility = itm.eligibilityName;
                    rep.origApptDate = itm.origApptDate;
                    rep.lastPromDate = itm.lastPromDate;
                    rep.empStatus = itm.empStatus;
                    rep.remarks = itm.EIC == null ? "VACANT" : itm.remarks;

                    rep.clusterCode = item.structureCode;
                    rep.clusterName = clusterName;

                    rep.divisionCode = itm.divisionCode;
                    rep.divisionName = divName;
                    rep.sectionCode = itm.sectionCode;
                    rep.sectionName = sectionName;
                    rep.unitCode = itm.unitCode;
                    rep.unitName = unitName;

                    rep.plantillaNo = iCount;
                    rep.printCode = printCode;
                    db.tRSPZPlantillaCurrents.Add(rep);
                }
            }

            db.SaveChanges();

            string reportType = "PlantillaCurrent";

            if (printType == "VACANT")
            {
                reportType = "VANCANTPOSITIONS";
            }

            Session["ReportType"] = reportType;
            Session["PrintReport"] = printCode;

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult PlantillaCSCForm(string id, string code )
        {
            string printCode = "PRNCODE" + DateTime.Now.ToString("yyMMddHHmmhhssfff");

            var tempList = Session["PlantillaCurrent"];
            IEnumerable<StructureParentViewModel> list = tempList as IEnumerable<StructureParentViewModel>;

            tOrgFunction dept = db.tOrgFunctions.Single(e => e.functionCode == id);

            int myYear = 2022;

            IEnumerable<PBOReportViewModel> ps = PersonnelServices(dept.departmentCode, myYear);

            int iCount = 0;
            List<vOrgStructure> tempStructChecker = new List<vOrgStructure>();

            foreach (StructureParentViewModel item in list)
            {
                foreach (PlantillaPosition itm in item.positionList)
                {
                    iCount = iCount + 1;
                    tRSPZPlantillaCurrent rep = new tRSPZPlantillaCurrent();


                    decimal tmpAnnualAuthorize = itm.currentYearRate * 12;
                    decimal tmpAnnualActual = itm.currentYearRate * 12;
                    int tmpStep = 1;

                    if (itm.EIC != null)
                    {
                        PBOReportViewModel tmpPS = ps.Single(e => e.EIC == itm.EIC);
                        tmpAnnualAuthorize = itm.currentYearRate * 12;
                        tmpAnnualActual = tmpPS.annualSalary;
                        tmpStep = tmpPS.stepInc;
                    }

                    string clusterName = "";

                    int isStructClusExist = tempStructChecker.Where(e => e.structureCode == itm.clusterCode).Count();
                    if (isStructClusExist >= 1)
                    {
                        clusterName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.clusterCode
                        });
                        clusterName = itm.clusterName != null ? itm.clusterName.ToUpper() : itm.clusterName;
                    }

                    string divName = "";
                    int isStructExist = tempStructChecker.Where(e => e.structureCode == itm.divisionCode).Count();
                    if (isStructExist >= 1)
                    {
                        divName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.divisionCode
                        });
                        divName = itm.divisionName;
                    }

                    string sectionName = "";
                    int isStructExistSec = tempStructChecker.Where(e => e.structureCode == itm.sectionCode).Count();
                    if (isStructExistSec >= 1)
                    {
                        sectionName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.sectionCode
                        });
                        sectionName = itm.sectionName;
                    }

                    string unitName = "";
                    int isStructExistUnit = tempStructChecker.Where(e => e.structureCode == itm.unitCode).Count();
                    if (isStructExistUnit >= 1)
                    {
                        unitName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.unitCode
                        });
                        unitName = itm.unitName;
                    }

                    string tmpLevel = "";
                    if (itm.positionLevel == 1)
                    {
                        tmpLevel = "1st";
                    }
                    else if (itm.positionLevel == 2)
                    {
                        tmpLevel = "2nd";
                    }

                    rep.functionCode = id;
                    rep.EIC = itm.EIC;
                    //rep.budgetYear = 2021;
                    rep.plantillaCode = itm.plantillaCode;
                    rep.oldItemNo = Convert.ToInt16(itm.oldItemNo).ToString("0000");
                    rep.itemNo = Convert.ToInt16(itm.itemNo).ToString("0000");
                    rep.salary = itm.currentYearRate;

                    rep.annualAuthorize = tmpAnnualAuthorize;
                    rep.annualActual = tmpAnnualActual;

                    rep.positionTitle = itm.positionTitle;
                    rep.subPositionTitle = itm.subPositionTitle;
                    rep.salaryGrade = itm.salaryGrade;
                    rep.step = tmpStep;
                    rep.positionLevel = tmpLevel;

                    rep.lastName = itm.lastName == null ? "" : itm.lastName.ToUpper();
                    rep.firstName = itm.firstName == null ? "" : itm.firstName.ToUpper();
                    rep.middleName = itm.middleName == null ? "" : itm.middleName.ToUpper();
                    rep.extname = itm.extName == null ? "" : itm.extName.ToUpper();

                    rep.birthDate = itm.birthDate;
                    rep.eligibility = itm.eligibilityName;
                    rep.origApptDate = itm.origApptDate;
                    rep.lastPromDate = itm.lastPromDate;
                    rep.empStatus = itm.empStatus;
                    rep.remarks = itm.EIC == null ? "VACANT" : itm.remarks;

                    rep.clusterCode = item.structureCode;
                    rep.clusterName = clusterName;

                    rep.divisionCode = itm.divisionCode;
                    rep.divisionName = divName;
                    rep.sectionCode = itm.sectionCode;
                    rep.sectionName = sectionName;
                    rep.unitCode = itm.unitCode;
                    rep.unitName = unitName;

                    rep.plantillaNo = iCount;
                    rep.printCode = printCode;
                    db.tRSPZPlantillaCurrents.Add(rep);
                }
            }
            db.SaveChanges();

            string rptType = "PlantillaCSCForm";
            if (code == "DBM")
            {
                rptType = "LBPFORM3A";
            }

            Session["ReportType"] = rptType;
            Session["PrintReport"] = printCode;

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult PlantillaDBMForm(string id)
        {
            string printCode = "PRNCODE" + DateTime.Now.ToString("yyMMddHHmmhhssfff");

            var tempList = Session["PlantillaCurrent"];
            IEnumerable<StructureParentViewModel> list = tempList as IEnumerable<StructureParentViewModel>;

            IEnumerable<PBOReportViewModel> ps = PersonnelServices(id, 2022);

            int iCount = 0;
            List<vOrgStructure> tempStructChecker = new List<vOrgStructure>();

            foreach (StructureParentViewModel item in list)
            {
                foreach (PlantillaPosition itm in item.positionList)
                {
                    iCount = iCount + 1;
                    tRSPZPlantillaCurrent rep = new tRSPZPlantillaCurrent();


                    decimal tmpAnnualAuthorize = itm.currentYearRate * 12;
                    decimal tmpAnnualActual = itm.currentYearRate * 12;
                    int tmpStep = 1;

                    if (itm.EIC != null)
                    {
                        PBOReportViewModel tmpPS = ps.Single(e => e.EIC == itm.EIC);
                        tmpAnnualAuthorize = itm.currentYearRate * 12;
                        tmpAnnualActual = tmpPS.annualSalary;
                        tmpStep = tmpPS.stepInc;
                    }

                    string clusterName = "";

                    int isStructClusExist = tempStructChecker.Where(e => e.structureCode == itm.clusterCode).Count();
                    if (isStructClusExist >= 1)
                    {
                        clusterName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.clusterCode
                        });
                        clusterName = itm.clusterName != null ? itm.clusterName.ToUpper() : itm.clusterName;
                    }

                    string divName = "";
                    int isStructExist = tempStructChecker.Where(e => e.structureCode == itm.divisionCode).Count();
                    if (isStructExist >= 1)
                    {
                        divName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.divisionCode
                        });
                        divName = itm.divisionName;
                    }

                    string sectionName = "";
                    int isStructExistSec = tempStructChecker.Where(e => e.structureCode == itm.sectionCode).Count();
                    if (isStructExistSec >= 1)
                    {
                        sectionName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.sectionCode
                        });
                        sectionName = itm.sectionName;
                    }

                    string unitName = "";
                    int isStructExistUnit = tempStructChecker.Where(e => e.structureCode == itm.unitCode).Count();
                    if (isStructExistUnit >= 1)
                    {
                        unitName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.unitCode
                        });
                        unitName = itm.unitName;
                    }

                    string tmpLevel = "";
                    if (itm.positionLevel == 1)
                    {
                        tmpLevel = "1st";
                    }
                    else if (itm.positionLevel == 2)
                    {
                        tmpLevel = "2nd";
                    }

                    rep.functionCode = id;
                    rep.EIC = itm.EIC;
                    //rep.budgetYear = 2021;
                    rep.plantillaCode = itm.plantillaCode;
                    rep.oldItemNo = Convert.ToInt16(itm.oldItemNo).ToString("0000");
                    rep.itemNo = Convert.ToInt16(itm.itemNo).ToString("0000");
                    rep.salary = itm.currentYearRate;

                    rep.annualAuthorize = tmpAnnualAuthorize;
                    rep.annualActual = tmpAnnualActual;

                    rep.positionTitle = itm.positionTitle;
                    rep.subPositionTitle = itm.subPositionTitle;
                    rep.salaryGrade = itm.salaryGrade;
                    rep.step = tmpStep;
                    rep.positionLevel = tmpLevel;

                    rep.lastName = itm.lastName == null ? "" : itm.lastName.ToUpper();
                    rep.firstName = itm.firstName == null ? "" : itm.firstName.ToUpper();
                    rep.middleName = itm.middleName == null ? "" : itm.middleName.ToUpper();
                    rep.extname = itm.extName == null ? "" : itm.extName.ToUpper();

                    rep.birthDate = itm.birthDate;
                    rep.eligibility = itm.eligibilityName;
                    rep.origApptDate = itm.origApptDate;
                    rep.lastPromDate = itm.lastPromDate;
                    rep.empStatus = itm.empStatus;
                    rep.remarks = itm.EIC == null ? "VACANT" : itm.remarks;

                    rep.clusterCode = item.structureCode;
                    rep.clusterName = clusterName;

                    rep.divisionCode = itm.divisionCode;
                    rep.divisionName = divName;
                    rep.sectionCode = itm.sectionCode;
                    rep.sectionName = sectionName;
                    rep.unitCode = itm.unitCode;
                    rep.unitName = unitName;

                    rep.plantillaNo = iCount;
                    rep.printCode = printCode;
                    db.tRSPZPlantillaCurrents.Add(rep);
                }
            }
             db.SaveChanges();

            Session["ReportType"] = "PlantillaDBMForm";
            Session["PrintReport"] = printCode;

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////////////////////
        /// UNFUNDED
        /// 

        [HttpPost]
        public JsonResult ShowUnfundedPlantilla(string id)
        {
            try
            {
                IEnumerable<StructureParentViewModel> list = DeptCurrentStructureByOfficeCode(id, false);
                Session["PlantillaUnfunded"] = list;
                return Json(new { status = "success", plantilla = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult UnfundedPlantillaCSCForm(string id, string code)
        {
            string printCode = "PRNCODE" + DateTime.Now.ToString("yyMMddHHmmhhssfff");

            var tempList = Session["PlantillaUnfunded"];
            IEnumerable<StructureParentViewModel> list = tempList as IEnumerable<StructureParentViewModel>;

            IEnumerable<PBOReportViewModel> ps = PersonnelServices(id, 2022);

            int iCount = 0;
            List<vOrgStructure> tempStructChecker = new List<vOrgStructure>();

            foreach (StructureParentViewModel item in list)
            {
                foreach (PlantillaPosition itm in item.positionList)
                {
                    iCount = iCount + 1;
                    tRSPZPlantillaCurrent rep = new tRSPZPlantillaCurrent();


                    decimal tmpAnnualAuthorize = itm.currentYearRate * 12;
                    decimal tmpAnnualActual = itm.currentYearRate * 12;
                    int tmpStep = 1;

                    if (itm.EIC != null)
                    {
                        PBOReportViewModel tmpPS = ps.Single(e => e.EIC == itm.EIC);
                        tmpAnnualAuthorize = itm.currentYearRate * 12;
                        tmpAnnualActual = tmpPS.annualSalary;
                        tmpStep = tmpPS.stepInc;
                    }

                    string clusterName = "";

                    int isStructClusExist = tempStructChecker.Where(e => e.structureCode == itm.clusterCode).Count();
                    if (isStructClusExist >= 1)
                    {
                        clusterName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.clusterCode
                        });
                        clusterName = itm.clusterName != null ? itm.clusterName.ToUpper() : itm.clusterName;
                    }

                    string divName = "";
                    int isStructExist = tempStructChecker.Where(e => e.structureCode == itm.divisionCode).Count();
                    if (isStructExist >= 1)
                    {
                        divName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.divisionCode
                        });
                        divName = itm.divisionName;
                    }

                    string sectionName = "";
                    int isStructExistSec = tempStructChecker.Where(e => e.structureCode == itm.sectionCode).Count();
                    if (isStructExistSec >= 1)
                    {
                        sectionName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.sectionCode
                        });
                        sectionName = itm.sectionName;
                    }

                    string unitName = "";
                    int isStructExistUnit = tempStructChecker.Where(e => e.structureCode == itm.unitCode).Count();
                    if (isStructExistUnit >= 1)
                    {
                        unitName = null;
                    }
                    else
                    {
                        tempStructChecker.Add(new vOrgStructure()
                        {
                            structureCode = itm.unitCode
                        });
                        unitName = itm.unitName;
                    }

                    string tmpLevel = "";
                    if (itm.positionLevel == 1)
                    {
                        tmpLevel = "1st";
                    }
                    else if (itm.positionLevel == 2)
                    {
                        tmpLevel = "2nd";
                    }

                    rep.functionCode = id;
                    rep.EIC = itm.EIC;
                    //rep.budgetYear = 2021;
                    rep.plantillaCode = itm.plantillaCode;
                    rep.oldItemNo = Convert.ToInt16(itm.oldItemNo).ToString("0000");
                    rep.itemNo = Convert.ToInt16(itm.itemNo).ToString("0000");
                    rep.salary = itm.currentYearRate;

                    rep.annualAuthorize = tmpAnnualAuthorize;
                    rep.annualActual = tmpAnnualActual;

                    rep.positionTitle = itm.positionTitle;
                    rep.subPositionTitle = itm.subPositionTitle;
                    rep.salaryGrade = itm.salaryGrade;
                    rep.step = tmpStep;
                    rep.positionLevel = tmpLevel;

                    rep.lastName = itm.lastName == null ? "" : itm.lastName.ToUpper();
                    rep.firstName = itm.firstName == null ? "" : itm.firstName.ToUpper();
                    rep.middleName = itm.middleName == null ? "" : itm.middleName.ToUpper();
                    rep.extname = itm.extName == null ? "" : itm.extName.ToUpper();

                    rep.birthDate = itm.birthDate;
                    rep.eligibility = itm.eligibilityName;
                    rep.origApptDate = itm.origApptDate;
                    rep.lastPromDate = itm.lastPromDate;
                    rep.empStatus = itm.empStatus;
                    rep.remarks = itm.EIC == null ? "VACANT" : itm.remarks;

                    rep.clusterCode = item.structureCode;
                    rep.clusterName = clusterName;

                    rep.divisionCode = itm.divisionCode;
                    rep.divisionName = divName;
                    rep.sectionCode = itm.sectionCode;
                    rep.sectionName = sectionName;
                    rep.unitCode = itm.unitCode;
                    rep.unitName = unitName;

                    rep.plantillaNo = iCount;
                    rep.printCode = printCode;
                    db.tRSPZPlantillaCurrents.Add(rep);

                }
            }
            db.SaveChanges();

            string rptType = "PlantillaCSCUForm";
            if (code == "DBM")
            {
                rptType = "LBPFORM3A";
                rptType = "PlantillaUnfunded";
            }

            Session["ReportType"] = rptType;
            Session["PrintReport"] = printCode;

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        /// ///////////////////////////////////////////////////////////////////////////////////////////////


        private IEnumerable<StructureParentViewModel> DeptCurrentStructure(string id, int tag)
        {

            tOrgFunction dept = db.tOrgFunctions.Single(e => e.functionCode == id);
            IEnumerable<vOrgStructure> struc = db.vOrgStructures.Where(e => e.departmentCode == dept.departmentCode).ToList();
            //IEnumerable<vRSPPlantillaProposed> deptPositionListPrinting = db.vRSPPlantillaProposeds.Where(e => e.functionCode == id && e.isFunded == false).OrderBy(o => o.plantillaNo).ToList();
            IEnumerable<vRSPPlantilla> deptPositionListPrinting;

            if (tag == -1)
            {
                deptPositionListPrinting = db.vRSPPlantillas.Where(e => e.functionCode == id && e.isFunded == false && e.fundStat == 0).OrderBy(o => o.plantillaNo).ToList();
            }
            else if (tag == 0)
            {
                deptPositionListPrinting = db.vRSPPlantillas.Where(e => e.functionCode == id && e.isFunded == true && e.EIC == null).OrderBy(o => o.plantillaNo).ToList();
            }
            else
            {
                deptPositionListPrinting = db.vRSPPlantillas.Where(e => e.functionCode == id && e.isFunded == true).OrderBy(o => o.plantillaNo).ToList();
            }

            //if (tag == 0)
            //{
            //    deptPositionListPrinting = deptPositionListPrinting.Where(e => e.EIC == null).ToList();
            //}

            // -1 UNFUNDED
            //  0 VACANT
            //  1 FUNDED 

            IEnumerable<PBOReportViewModel> ps = PersonnelServices(dept.departmentCode, 0);


            List<StructureParentViewModel> finList = new List<StructureParentViewModel>();
            List<vOrgStructure> structureChecker = new List<vOrgStructure>();
            string prevStructCode = "";
            int counter = 0;

            IEnumerable<tRSPSalaryTableDetail> sTable = db.tRSPSalaryTableDetails.Where(e => e.salaryCode == "MSSLT12021").ToList();

            List<vRSPPlantillaProposed> printPreviewer = new List<vRSPPlantillaProposed>();
            foreach (vRSPPlantilla item in deptPositionListPrinting)
            {
                counter = counter + 1;
                string newHeaderCode = "";
                if (item.structureCode != prevStructCode)
                {
                    //CHECK IF SCTRUCTURE CODE EXIST
                    vOrgStructure checker = structureChecker.FirstOrDefault(e => e.structureCode == item.structureCode);

                    if (checker != null)
                    {
                        //ADD BLANK CHECKER
                        string tmpCode = "GROUP" + String.Format("{0:00000}", counter);
                        structureChecker.Add(new vOrgStructure()
                        {
                            structureCode = tmpCode,
                            //strucNo = item.strucNo
                        });
                        newHeaderCode = tmpCode;
                    }
                    else
                    {
                        //ADD NEW ITEM TO CHECKER
                        structureChecker.Add(new vOrgStructure()
                        {
                            structureCode = item.structureCode,
                            //strucNo = item.strucNo
                        });
                        newHeaderCode = item.structureCode;
                    }

                }
                else if (item.structureCode == prevStructCode)
                {
                    //ADD TO THE GROUP
                    newHeaderCode = item.structureCode;
                }

                if (item.EIC == null)
                {
                    item.rateMonth = Convert.ToDecimal(sTable.Where(e => e.salaryGrade == item.salaryGrade && e.step == 1).First().rateMonth);
                }


                printPreviewer.Add(new vRSPPlantillaProposed()
                {
                    plantillaCode = item.plantillaCode,
                    itemNo = item.itemNo,
                    oldItemNo = item.oldItemNo,
                    rateCurrent = item.rateMonth,
                    positionTitle = item.positionTitle,
                    subPositionTitle = item.subPositionTitle,
                    salaryGrade = item.salaryGrade,
                    step = item.step == 0 ? 1 : item.step,
                    positionLevel = item.positionLevel,
                    EIC = item.EIC,
                    fullNameLast = item.fullNameLast,
                    lastName = item.lastName,
                    firstName = item.firstName,
                    middleName = item.middleName,
                    extName = item.extName,
                    birthDate = item.birthDate,
                    isFunded = item.isFunded,
                    fundStat = item.fundStat,
                    eligibilityName = item.eligibilityNameShort,
                    dateOrigAppointment = item.dateOrigAppointment,
                    dateLastPromoted = item.dateLastPromoted,
                    employmentStatus = item.employmentStatusNameShort,
                    //remarks = item.remarks,
                    departmentName = item.departmentName,
                    clusterCode = item.clusterCode,
                    clusterName = item.clusterName,
                    divisionCode = item.divisionCode,
                    divisionName = item.divisionName,
                    sectionCode = item.sectionCode,
                    sectionName = item.sectionName,
                    unitCode = item.unitCode,
                    unitName = item.unitName,
                    //strucNo = item.strucNo,
                    structureCode = newHeaderCode
                });

                //SET NEW PREVIOUS CODE
                prevStructCode = item.structureCode;
            }

            var groupByPosition = printPreviewer.GroupBy(e => e.structureCode).Select(g => new { g.Key });


            foreach (var item in groupByPosition)
            {
                string s = item.Key;

                vOrgStructure nod = struc.SingleOrDefault(e => e.structureCode == s);

                string strucName = "";
                string parentPath = "";

                if (nod == null)
                {
                    nod = structureChecker.Single(e => e.structureCode == s);
                }
                else
                {
                    strucName = structName(nod);
                    parentPath = parentAddress(nod);
                }

                finList.Add(new StructureParentViewModel()
                {
                    structureCode = s,
                    structNo = Convert.ToInt16(nod.strucNo),
                    structureName = strucName,
                    structurePath = parentPath + "\\" + strucName,
                    parentPath = parentPath,
                    parentNo = Convert.ToInt16(nod.strucNo),
                    levelNo = 0,
                    orderNo = Convert.ToInt16(0),
                    positionList = PositionList2(s, printPreviewer, ps).ToList()
                });
            }

            return finList;

        }


        private IEnumerable<StructureParentViewModel> DeptCurrentStructureByOfficeCode(string id, bool isFunded)
        {

            tOrgDepartment dept = db.tOrgDepartments.Single(e => e.departmentCode == id);

            IEnumerable<vOrgStructure> struc = db.vOrgStructures.Where(e => e.departmentCode == dept.departmentCode).ToList();

            IEnumerable<vRSPPlantilla> deptPositionListPrinting;

            if (isFunded == true)
            {
                deptPositionListPrinting = db.vRSPPlantillas.Where(e => e.departmentCode == id && e.isFunded == true).OrderBy(o => o.itemNo).ToList();
            }
            else
            {
                deptPositionListPrinting = db.vRSPPlantillas.Where(e => e.departmentCode == id && e.isFunded == false).OrderBy(o => o.plantillaNo).ToList();
            }

            List<StructureParentViewModel> finList = new List<StructureParentViewModel>();
            List<vOrgStructure> structureChecker = new List<vOrgStructure>();
            string prevStructCode = "";
            int counter = 0;

            IEnumerable<PBOReportViewModel> ps = PersonnelServices(id, 0);

            List<vRSPPlantillaProposed> printPreviewer = new List<vRSPPlantillaProposed>();
            foreach (vRSPPlantilla item in deptPositionListPrinting)
            {
                counter = counter + 1;
                string newHeaderCode = "";
                if (item.structureCode != prevStructCode)
                {
                    //CHECK IF SCTRUCTURE CODE EXIST
                    vOrgStructure checker = structureChecker.FirstOrDefault(e => e.structureCode == item.structureCode);

                    if (checker != null)
                    {
                        //ADD BLANK CHECKER
                        string tmpCode = "GROUP" + String.Format("{0:00000}", counter);
                        structureChecker.Add(new vOrgStructure()
                        {
                            structureCode = tmpCode,
                            //strucNo = item.strucNo
                        });
                        newHeaderCode = tmpCode;
                    }
                    else
                    {
                        //ADD NEW ITEM TO CHECKER
                        structureChecker.Add(new vOrgStructure()
                        {
                            structureCode = item.structureCode,
                            //strucNo = item.strucNo
                        });
                        newHeaderCode = item.structureCode;
                    }

                }
                else if (item.structureCode == prevStructCode)
                {
                    //ADD TO THE GROUP
                    newHeaderCode = item.structureCode;
                }


                if (item.itemNo == 997)
                {
                    item.itemNo = 997;
                }

                printPreviewer.Add(new vRSPPlantillaProposed()
                {
                    plantillaCode = item.plantillaCode,
                    itemNo = item.itemNo,
                    oldItemNo = item.oldItemNo,
                    rateCurrent = item.rateMonth,
                    positionTitle = item.positionTitle,
                    subPositionTitle = item.subPositionTitle,
                    salaryGrade = item.salaryGrade,
                    step = item.step == 0 ? 1 : item.step,
                    positionLevel = item.positionLevel,
                    EIC = item.EIC,
                    fullNameLast = item.fullNameLast,
                    lastName = item.lastName,
                    firstName = item.firstName,
                    middleName = item.middleName,
                    extName = item.extName,
                    birthDate = item.birthDate,
                    isFunded = item.isFunded,
                    eligibilityName = item.eligibilityNameShort,
                    dateOrigAppointment = item.dateOrigAppointment,
                    dateLastPromoted = item.dateLastPromoted,
                    employmentStatus = item.employmentStatusNameShort,
                    departmentName = item.departmentName,
                    clusterCode = item.clusterCode,
                    clusterName = item.clusterName,
                    divisionCode = item.divisionCode,
                    divisionName = item.divisionName,
                    sectionCode = item.sectionCode,
                    sectionName = item.sectionName,
                    unitCode = item.unitCode,
                    unitName = item.unitName,
                    structureCode = newHeaderCode
                });
                //SET NEW PREVIOUS CODE
                prevStructCode = item.structureCode;
            }

            var groupByPosition = printPreviewer.GroupBy(e => e.structureCode).Select(g => new { g.Key });


            foreach (var item in groupByPosition)
            {
                string s = item.Key;

                vOrgStructure nod = struc.SingleOrDefault(e => e.structureCode == s);

                string strucName = "";
                string parentPath = "";

                if (nod == null)
                {
                    nod = structureChecker.Single(e => e.structureCode == s);
                }
                else
                {
                    strucName = structName(nod);
                    parentPath = parentAddress(nod);
                }

                finList.Add(new StructureParentViewModel()
                {
                    structureCode = s,
                    structNo = Convert.ToInt16(nod.strucNo),
                    structureName = strucName,
                    structurePath = parentPath + "\\" + strucName,
                    parentPath = parentPath,
                    parentNo = Convert.ToInt16(nod.strucNo),
                    levelNo = 0,
                    orderNo = Convert.ToInt16(0),
                    positionList = PositionList2(s, printPreviewer, ps).ToList()
                });
            }

            return finList;

        }


        private IEnumerable<StructureParentViewModel> DeptCurrentStructureByFunctionCode(string id, bool isFunded)
        {

            tOrgFunction dept = db.tOrgFunctions.Single(e => e.functionCode == id);
            //tOrgDepartment dept = db.tOrgDepartments.Single(e => e.departmentCode == id);

            IEnumerable<vOrgStructure> struc = db.vOrgStructures.Where(e => e.departmentCode == dept.departmentCode).ToList();

            IEnumerable<vRSPPlantilla> deptPositionListPrinting;

            if (isFunded == true)
            {
                deptPositionListPrinting = db.vRSPPlantillas.Where(e => e.departmentCode == id && e.isFunded == true).OrderBy(o => o.itemNo).ToList();
            }
            else
            {
                deptPositionListPrinting = db.vRSPPlantillas.Where(e => e.departmentCode == id && e.isFunded == false).OrderBy(o => o.plantillaNo).ToList();
            }

            List<StructureParentViewModel> finList = new List<StructureParentViewModel>();
            List<vOrgStructure> structureChecker = new List<vOrgStructure>();
            string prevStructCode = "";
            int counter = 0;

            IEnumerable<PBOReportViewModel> ps = PersonnelServices(id, 0);

            List<vRSPPlantillaProposed> printPreviewer = new List<vRSPPlantillaProposed>();
            foreach (vRSPPlantilla item in deptPositionListPrinting)
            {
                counter = counter + 1;
                string newHeaderCode = "";
                if (item.structureCode != prevStructCode)
                {
                    //CHECK IF SCTRUCTURE CODE EXIST
                    vOrgStructure checker = structureChecker.FirstOrDefault(e => e.structureCode == item.structureCode);

                    if (checker != null)
                    {
                        //ADD BLANK CHECKER
                        string tmpCode = "GROUP" + String.Format("{0:00000}", counter);
                        structureChecker.Add(new vOrgStructure()
                        {
                            structureCode = tmpCode,
                            //strucNo = item.strucNo
                        });
                        newHeaderCode = tmpCode;
                    }
                    else
                    {
                        //ADD NEW ITEM TO CHECKER
                        structureChecker.Add(new vOrgStructure()
                        {
                            structureCode = item.structureCode,
                            //strucNo = item.strucNo
                        });
                        newHeaderCode = item.structureCode;
                    }

                }
                else if (item.structureCode == prevStructCode)
                {
                    //ADD TO THE GROUP
                    newHeaderCode = item.structureCode;
                }

                printPreviewer.Add(new vRSPPlantillaProposed()
                {
                    plantillaCode = item.plantillaCode,
                    itemNo = item.itemNo,
                    oldItemNo = item.oldItemNo,
                    rateCurrent = item.rateMonth,
                    positionTitle = item.positionTitle,
                    subPositionTitle = item.subPositionTitle,
                    salaryGrade = item.salaryGrade,
                    step = item.step == 0 ? 1 : item.step,
                    positionLevel = item.positionLevel,
                    EIC = item.EIC,
                    fullNameLast = item.fullNameLast,
                    lastName = item.lastName,
                    firstName = item.firstName,
                    middleName = item.middleName,
                    extName = item.extName,
                    birthDate = item.birthDate,
                    isFunded = item.isFunded,
                    eligibilityName = item.eligibilityNameShort,
                    dateOrigAppointment = item.dateOrigAppointment,
                    dateLastPromoted = item.dateLastPromoted,
                    employmentStatus = item.employmentStatusNameShort,
                    departmentName = item.departmentName,
                    clusterCode = item.clusterCode,
                    clusterName = item.clusterName,
                    divisionCode = item.divisionCode,
                    divisionName = item.divisionName,
                    sectionCode = item.sectionCode,
                    sectionName = item.sectionName,
                    unitCode = item.unitCode,
                    unitName = item.unitName,
                    structureCode = newHeaderCode
                });
                //SET NEW PREVIOUS CODE
                prevStructCode = item.structureCode;
            }

            var groupByPosition = printPreviewer.GroupBy(e => e.structureCode).Select(g => new { g.Key });


            foreach (var item in groupByPosition)
            {
                string s = item.Key;

                vOrgStructure nod = struc.SingleOrDefault(e => e.structureCode == s);

                string strucName = "";
                string parentPath = "";

                if (nod == null)
                {
                    nod = structureChecker.Single(e => e.structureCode == s);
                }
                else
                {
                    strucName = structName(nod);
                    parentPath = parentAddress(nod);
                }

                finList.Add(new StructureParentViewModel()
                {
                    structureCode = s,
                    structNo = Convert.ToInt16(nod.strucNo),
                    structureName = strucName,
                    structurePath = parentPath + "\\" + strucName,
                    parentPath = parentPath,
                    parentNo = Convert.ToInt16(nod.strucNo),
                    levelNo = 0,
                    orderNo = Convert.ToInt16(0),
                    positionList = PositionList2(s, printPreviewer, ps).ToList()
                });
            }

            return finList;

        }

        public ActionResult Proposed()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ProposedInitData()
        {
            try
            {
                IEnumerable<tOrgFunction> funcList = db.tOrgFunctions.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();
                //IEnumerable<tOrgDepartment> deptList = db.tOrgDepartments.Where(e => e.isActive == true).OrderBy(o => o.orderNo).ToList();
                List<tOrgFunction> departmentList = new List<tOrgFunction>();
                foreach (tOrgFunction p in funcList)
                {
                    departmentList.Add(new tOrgFunction()
                    {
                        functionCode = p.functionCode,
                        departmentName = p.functionDesc
                    });
                }
                return Json(new { status = "success", functionList = departmentList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        //NG
        [HttpPost]
        public JsonResult PrintPlantillaSetup(string id, string type)
        {

            int recCount = db.tReportPlantillas.Where(e => e.functionCode == id && e.budgetYear == 2022).Count();
            if (recCount == 0)
            {
                IEnumerable<StructureParentViewModel> list = DeptProposedStructure(id, 2022);
            }

            //tOrgFunction deptFunc = db.tOrgFunctions.Single(e => e.functionCode == id);
            string reportType = "";
            if (type == "LBPFORM3")
            {
                reportType = "LBPFORM3";
            }
            else if (type == "LBPFORM3A")
            {
                reportType = "LBPFORM3A";
            }
            else
            {
                reportType = "PlantillaProposed";
            }

            Session["ReportType"] = reportType;
            Session["PrintReport"] = id;

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }

       


        //NG


        //USE FOR PLANTILLA -PROPOSED
        private IEnumerable<StructureParentViewModel> DeptProposedStructure(string id, int propBudgetYear)
        {
            tOrgFunction dept = db.tOrgFunctions.Single(e => e.functionCode == id);
            IEnumerable<vOrgStructure> struc = db.vOrgStructures.Where(e => e.departmentCode == dept.departmentCode).ToList();
            IEnumerable<vRSPPlantillaProposed> deptPositionListPrinting = db.vRSPPlantillaProposeds.Where(e => e.functionCode == id).OrderBy(o => o.plantillaNo).ToList();

            List<StructureParentViewModel> finList = new List<StructureParentViewModel>();
            List<vOrgStructure> structureChecker = new List<vOrgStructure>();
            string prevStructCode = "";
            int counter = 0;

            //IEnumerable<PBOReportViewModel> ps = PersonnelServices(id, propBudgetYear);

            IEnumerable<tReportBudgetaryReq> ps = db.tReportBudgetaryReqs.Where(e => e.departmentCode == dept.departmentCode && e.budgetYear == propBudgetYear).ToList();
            
            List<vRSPPlantillaProposed> printPreviewer = new List<vRSPPlantillaProposed>();
            foreach (vRSPPlantillaProposed item in deptPositionListPrinting)
            {
                counter = counter + 1;
                string newHeaderCode = "";
                if (item.structureCode != prevStructCode)
                {
                    //CHECK IF SCTRUCTURE CODE EXIST
                    vOrgStructure checker = structureChecker.FirstOrDefault(e => e.structureCode == item.structureCode);

                    if (checker != null)
                    {
                        //ADD BLANK CHECKER
                        string tmpCode = "GROUP" + String.Format("{0:00000}", counter);
                        structureChecker.Add(new vOrgStructure()
                        {
                            structureCode = tmpCode,
                            strucNo = item.strucNo
                        });
                        newHeaderCode = tmpCode;
                    }
                    else
                    {
                        //ADD NEW ITEM TO CHECKER
                        structureChecker.Add(new vOrgStructure()
                        {
                            structureCode = item.structureCode,
                            strucNo = item.strucNo
                        });
                        newHeaderCode = item.structureCode;
                    }

                }
                else if (item.structureCode == prevStructCode)
                {
                    //ADD TO THE GROUP
                    newHeaderCode = item.structureCode;
                }

                


                printPreviewer.Add(new vRSPPlantillaProposed()
                {
                    plantillaCode = item.plantillaCode,
                    itemNo = item.tmpItemNo,
                    oldItemNo = item.itemNo,
                    rateCurrent = item.rateCurrent,
                    rateProposed = item.rateProposed,
                    positionTitle = item.positionTitle,
                    subPositionTitle = item.subPositionTitle,
                    salaryGrade = item.salaryGrade,
                    step = item.step == 0 ? 1 : item.step,
                    positionLevel = item.positionLevel,
                    EIC = item.EIC,
                    fullNameLast = item.fullNameLast,
                    lastName = item.lastName,
                    firstName = item.firstName,
                    middleName = item.middleName,
                    extName = item.extName,
                    birthDate = item.birthDate,
                    isFunded = item.isFunded,
                    fundStat = item.fundStat,
                    eligibilityName = item.eligibilityNameShort,
                    dateOrigAppointment = item.dateOrigAppointment,
                    dateLastPromoted = item.dateLastPromoted,
                    employmentStatus = item.employmentStatusNameShort,
                    remarks = item.remarks,
                    departmentName = item.departmentName,
                    clusterCode = item.clusterCode,
                    clusterName = item.clusterName,
                    divisionCode = item.divisionCode,
                    divisionName = item.divisionName,
                    sectionCode = item.sectionCode,
                    sectionName = item.sectionName,
                    unitCode = item.unitCode,
                    unitName = item.unitName,
                    strucNo = item.strucNo,
                    structureCode = newHeaderCode,
                    plantillaNo = item.plantillaNo
                });

                //SET NEW PREVIOUS CODE
                prevStructCode = item.structureCode;
            }

            var groupByPosition = printPreviewer.GroupBy(e => e.structureCode).Select(g => new { g.Key });


            foreach (var item in groupByPosition)
            {
                string s = item.Key;

                vOrgStructure nod = struc.SingleOrDefault(e => e.structureCode == s);

                string strucName = "";
                string parentPath = "";

                if (nod == null)
                {
                    nod = structureChecker.Single(e => e.structureCode == s);
                }
                else
                {
                    strucName = structName(nod);
                    parentPath = parentAddress(nod);
                }

                finList.Add(new StructureParentViewModel()
                {
                    structureCode = s,
                    structNo = Convert.ToInt16(nod.strucNo),
                    structureName = strucName,
                    structurePath = parentPath + "\\" + strucName,
                    parentPath = parentPath,
                    parentNo = Convert.ToInt16(nod.strucNo),
                    levelNo = 0,
                    orderNo = Convert.ToInt16(0),
                    positionList = PositionList3(s, printPreviewer, ps).ToList()
                });
            }


            //ADD SAVING CODE FOR PRINTING
            int rCount = db.tReportPlantillas.Where(e => e.functionCode == id && e.budgetYear == 2022).Count();

            if (rCount == 0)
            {
                //vOrgStructure t = structureChecker.FirstOrDefault(e => e.structureCode == item.structureCode);
                List<vOrgStructure> tempStructChecker = new List<vOrgStructure>();
                int iCount = 0;
                foreach (StructureParentViewModel item in finList)
                {
                    foreach (PlantillaPosition itm in item.positionList)
                    {

                        iCount = iCount + 1;
                        tReportPlantilla rep = new tReportPlantilla();

                        string clusterName = "";

                        int isStructClusExist = tempStructChecker.Where(e => e.structureCode == itm.clusterCode).Count();
                        if (isStructClusExist >= 1)
                        {
                            clusterName = null;
                        }
                        else
                        {
                            tempStructChecker.Add(new vOrgStructure()
                            {
                                structureCode = itm.clusterCode
                            });
                            clusterName = itm.clusterName != null ? itm.clusterName.ToUpper() : itm.clusterName;
                        }


                        string divName = "";
                        int isStructExist = tempStructChecker.Where(e => e.structureCode == itm.divisionCode).Count();
                        if (isStructExist >= 1)
                        {
                            divName = null;
                        }
                        else
                        {
                            tempStructChecker.Add(new vOrgStructure()
                            {
                                structureCode = itm.divisionCode
                            });
                            divName = itm.divisionName;
                        }

                        string sectionName = "";
                        int isStructExistSec = tempStructChecker.Where(e => e.structureCode == itm.sectionCode).Count();
                        if (isStructExistSec >= 1)
                        {
                            sectionName = null;
                        }
                        else
                        {
                            tempStructChecker.Add(new vOrgStructure()
                            {
                                structureCode = itm.sectionCode
                            });
                            sectionName = itm.sectionName;
                        }

                        string unitName = "";
                        int isStructExistUnit = tempStructChecker.Where(e => e.structureCode == itm.unitCode).Count();
                        if (isStructExistUnit >= 1)
                        {
                            unitName = null;
                        }
                        else
                        {
                            tempStructChecker.Add(new vOrgStructure()
                            {
                                structureCode = itm.unitCode
                            });
                            unitName = itm.unitName;
                        }

                        string tmpLevel = "";
                        if (itm.positionLevel == 1)
                        {
                            tmpLevel = "1st";
                        }
                        else if (itm.positionLevel == 2)
                        {
                            tmpLevel = "2nd";
                        }

                        rep.functionCode = id;
                        rep.budgetYear = propBudgetYear;
                        rep.oldItemNo = Convert.ToInt16(itm.oldItemNo);
                        rep.itemNo = Convert.ToInt16(itm.itemNo);
                        //rep.plantillaCode = *** SAVE PLANTILLA CODE PLEASE
                        rep.currentYearRate = Convert.ToDecimal(itm.currentYearRate);
                        rep.proposedYearRate = itm.proposeYearRate == 0 ? Convert.ToDecimal(itm.currentYearRate) : Convert.ToDecimal(itm.proposeYearRate);
                        rep.positionTitle = itm.positionTitle;
                        rep.subPositionTitle = itm.subPositionTitle;
                        rep.salaryGrade = itm.salaryGrade;
                        rep.step = itm.step == 0 ? 1 : itm.step;

                        rep.positionLevel = tmpLevel;
                        rep.lastName = itm.lastName;
                        rep.firstName = itm.firstName;
                        rep.middleName = itm.middleName;
                        rep.extnName = itm.extName;

                        rep.birthDate = itm.birthDate;
                        rep.eligibility = itm.eligibilityName;
                        rep.dateOrigAppointment = itm.origApptDate;
                        rep.dateLastPromoted = itm.lastPromDate;
                        rep.statusName = itm.empStatus;
                        rep.remarks = itm.EIC == null ? "VACANT" : itm.remarks;

                        rep.clusterCode = item.structureCode;
                        rep.clusterName = clusterName;

                        rep.divisionCode = itm.divisionCode;
                        rep.divisionName = divName;
                        rep.sectionCode = itm.sectionCode;
                        rep.sectionName = sectionName;
                        rep.unitCode = itm.unitCode;
                        rep.unitName = unitName;

                        rep.orderNo = iCount;
                        db.tReportPlantillas.Add(rep);
                    }
                }
                db.SaveChanges();
            }

            return finList;

        }


        private IEnumerable<PlantillaPosition> PositionList2(string structureCode, IEnumerable<vRSPPlantillaProposed> data, IEnumerable<PBOReportViewModel> psData)
        {

            List<PlantillaPosition> pos = new List<PlantillaPosition>();

            try
            {
                data = data.Where(e => e.structureCode == structureCode).OrderBy(o => o.plantillaNo).ToList();
                foreach (vRSPPlantillaProposed item in data)
                {

                    decimal tmpAnnualAuthorize = Convert.ToDecimal(item.rateCurrent);
                    decimal tmpAnnualActual = tmpAnnualAuthorize;

                    int stepInc = Convert.ToInt16(item.step);


                    if (item.EIC != null)
                    {
                        PBOReportViewModel tmpPS = psData.SingleOrDefault(e => e.EIC == item.EIC);
                        if (tmpPS != null)
                        {
                            tmpAnnualActual = tmpPS.annualSalary;
                            stepInc = tmpPS.stepInc;
                        }

                    }

                    if (stepInc == 0)
                    {
                        stepInc = 1;
                    }

                    pos.Add(new PlantillaPosition()
                    {

                        plantillaCode = item.plantillaCode,
                        oldItemNo = Convert.ToInt16(item.oldItemNo).ToString("0000"),
                        itemNo = Convert.ToInt16(item.itemNo).ToString("0000"),

                        currentYearRate = tmpAnnualAuthorize,
                        proposeYearRate = tmpAnnualActual,

                        positionTitle = item.positionTitle.ToUpper(),
                        positionLevel = Convert.ToInt16(item.positionLevel),

                        subPositionTitle = item.subPositionTitle != null ? item.subPositionTitle.ToUpper() : null,
                        salaryGrade = Convert.ToInt16(item.salaryGrade),
                        step = stepInc,
                        EIC = item.EIC,
                        lastName = item.lastName,
                        firstName = item.firstName,
                        middleName = item.middleName,
                        extName = item.extName,
                        fullNameLast = item.fullNameLast,
                        isFunded = Convert.ToInt16(item.isFunded),
                        fundStat = Convert.ToInt16(item.fundStat),
                        birthDate = Convert.ToDateTime(item.birthDate),
                        eligibilityName = item.eligibilityName,
                        origApptDate = Convert.ToDateTime(item.dateOrigAppointment),
                        lastPromDate = Convert.ToDateTime(item.dateLastPromoted),
                        empStatus = item.employmentStatus,
                        remarks = item.remarks,
                        clusterCode = item.clusterCode,
                        clusterName = item.clusterName,
                        divisionCode = item.divisionCode,
                        divisionName = item.divisionName,
                        sectionCode = item.sectionCode,
                        sectionName = item.sectionName,
                        unitCode = item.unitCode,
                        unitName = item.unitName,
                        plantillaNo = Convert.ToInt16(item.plantillaNo)
                    });
                }
                return pos;
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return pos;
            }
        }



        private IEnumerable<PlantillaPosition> PositionList3(string structureCode, IEnumerable<vRSPPlantillaProposed> data, IEnumerable<tReportBudgetaryReq> psData)
        {

            List<PlantillaPosition> pos = new List<PlantillaPosition>();

            string tmpErrorIndx = "";

            try
            {
             
                data = data.Where(e => e.structureCode == structureCode).OrderBy(o => o.plantillaNo).ToList();
                foreach (vRSPPlantillaProposed item in data)
                {

                    tmpErrorIndx = item.itemNo.ToString() + " " + item.plantillaNo.ToString();

                    decimal tmpAnnualAuthorize = Convert.ToDecimal(item.rateCurrent);
                    decimal tmpAnnualActual = tmpAnnualAuthorize;

                    int stepInc = Convert.ToInt16(item.step);
                    
                    if (item.EIC != null)
                    {
                        tReportBudgetaryReq tmpPS = psData.SingleOrDefault(e => e.EIC == item.EIC && e.plantillaCode == item.plantillaCode && e.budgetYear == 2022) ;
                        if (tmpPS != null)
                        {
                            tmpAnnualActual = Convert.ToDecimal(tmpPS.annualSalary);
                            stepInc = Convert.ToInt16(tmpPS.stepInc);
                        }

                    }

                    if (stepInc == 0)
                    {
                        stepInc = 1;
                    }

                    pos.Add(new PlantillaPosition()
                    {

                        plantillaCode = item.plantillaCode,
                        oldItemNo = Convert.ToInt16(item.oldItemNo).ToString("0000"),
                        itemNo = Convert.ToInt16(item.itemNo).ToString("0000"),

                        currentYearRate = tmpAnnualAuthorize,
                        proposeYearRate = tmpAnnualActual,

                        positionTitle = item.positionTitle.ToUpper(),
                        positionLevel = Convert.ToInt16(item.positionLevel),

                        subPositionTitle = item.subPositionTitle != null ? item.subPositionTitle.ToUpper() : null,
                        salaryGrade = Convert.ToInt16(item.salaryGrade),
                        step = stepInc,
                        EIC = item.EIC,
                        lastName = item.lastName,
                        firstName = item.firstName,
                        middleName = item.middleName,
                        extName = item.extName,
                        fullNameLast = item.fullNameLast,
                        isFunded = Convert.ToInt16(item.isFunded),
                        fundStat = Convert.ToInt16(item.fundStat),
                        birthDate = Convert.ToDateTime(item.birthDate),
                        eligibilityName = item.eligibilityName,
                        origApptDate = Convert.ToDateTime(item.dateOrigAppointment),
                        lastPromDate = Convert.ToDateTime(item.dateLastPromoted),
                        empStatus = item.employmentStatus,
                        remarks = item.remarks,
                        clusterCode = item.clusterCode,
                        clusterName = item.clusterName,
                        divisionCode = item.divisionCode,
                        divisionName = item.divisionName,
                        sectionCode = item.sectionCode,
                        sectionName = item.sectionName,
                        unitCode = item.unitCode,
                        unitName = item.unitName,
                        plantillaNo = Convert.ToInt16(item.plantillaNo)
                    });
                }
                return pos;
            }
            catch (Exception ex)
            {
                string errMsg = tmpErrorIndx;
                return pos;
            }
        }



        private string structName(vOrgStructure item)
        {
            string result = "";
            if (item.strucNo == 0)
            {
                result = item.departmentName;
            }
            else if (item.strucNo == 1)
            {
                result = item.clusterName;
            }
            else if (item.strucNo == 2)
            {
                result = item.divisionName;
            }
            else if (item.strucNo == 3)
            {
                result = item.sectionName;
            }
            else if (item.strucNo == 4)
            {
                result = item.unitName;
            }
            return result;
        }

        private string parentAddress(vOrgStructure item)
        {
            string res;

            res = item.shortDepartmentName;

            if (item.clusterCode != null)
            {
                res = res + "\\" + item.clusterName;
            }

            if (item.divisionCode != null)
            {
                res = res + "\\" + item.divisionName;
            }

            if (item.sectionCode != null)
            {
                res = res + "\\" + item.sectionName;
            }

            return res;
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // QUALIFICATION STANDARD
        [HttpPost]
        public JsonResult PositionQS(string id)
        {
            try
            {
                tRSPPositionQ qs = db.tRSPPositionQS.SingleOrDefault(e => e.plantillaCode == id);
                if (qs == null)
                {
                    tRSPPositionQ temp = new tRSPPositionQ();
                    temp.plantillaCode = id;
                    db.tRSPPositionQS.Add(temp);
                    db.SaveChanges();
                    qs = temp;
                }

                return Json(new { status = "success", qs = qs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult UpdateQStandard(tRSPPositionQ data)
        {
            try
            {
                tRSPPositionQ qs = db.tRSPPositionQS.Single(e => e.plantillaCode == data.plantillaCode);
                qs.QSEducation = data.QSEducation;
                qs.QSExperience = data.QSExperience;
                qs.QSTraining = data.QSTraining;
                qs.QSEligibility = data.QSEligibility;
                qs.QSEligibilityPub = data.QSEligibilityPub;
                qs.QSNotation = data.QSNotation;
                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);

            }
        }


        public class PositionJobDesc
        {
            public int recNo { get; set; }
            public string plantillaCode { get; set; }
            public string jobDescCode { get; set; }
            public int jobSeqNo { get; set; }
            public string jobDesc { get; set; }
            public int percentage { get; set; }
            public List<tRSPPositionJobDescSub> jobDescSubList { get; set; }

        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // JOB DESCRIPTION STANDARD
        [HttpPost]
        public JsonResult ViewJobDesc(string id)
        {
            try
            {

                List<PositionJobDesc> finalJD = _GetJobDescriptionList(id);

                ///IEnumerable<tRSPPositionJobDesc> jobDesc = db.tRSPPositionJobDescs.Where(e => e.plantillaCode == id).OrderBy(o => o.jobSeqNo).ToList();

                //if( jobDesc.First().jobDescCode == null) 
                //{
                //    GenerateJobDescCode(jobDesc);
                //}


                //IEnumerable<tRSPPositionJobDescSub> subJobDesc = db.tRSPPositionJobDescSubs.Where(e => e.recNo > 0).ToList();
                //List<PositionJobDesc> finalJD = new List<PositionJobDesc>();

                //foreach (tRSPPositionJobDesc item in jobDesc)
                //{
                //    finalJD.Add(new PositionJobDesc()
                //    {
                //        recNo = item.recNo,
                //        plantillaCode = item.plantillaCode,
                //        jobDescCode = item.jobDescCode,
                //        jobSeqNo = Convert.ToInt16(item.jobSeqNo),
                //        jobDesc = item.jobDesc,
                //        percentage = Convert.ToInt16(item.percentage),
                //        jobDescSubList = subJobDesc.Where(e => e.jobDescCode == item.jobDescCode).OrderBy(o => o.subJDNo).ToList()
                //    });
                //}
                return Json(new { status = "success", jobDesc = finalJD }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);

            }
        }

        private List<PositionJobDesc> _GetJobDescriptionList(string id)
        {
            List<PositionJobDesc> finalJD = new List<PositionJobDesc>();
            try
            {
                IEnumerable<tRSPPositionJobDesc> jobDesc = db.tRSPPositionJobDescs.Where(e => e.plantillaCode == id).OrderBy(o => o.jobSeqNo).ToList();
                IEnumerable<tRSPPositionJobDescSub> subJobDesc = db.tRSPPositionJobDescSubs.Where(e => e.recNo > 0).ToList();
                foreach (tRSPPositionJobDesc item in jobDesc)
                {
                    finalJD.Add(new PositionJobDesc()
                    {
                        recNo = item.recNo,
                        plantillaCode = item.plantillaCode,
                        jobDescCode = item.jobDescCode,
                        jobSeqNo = Convert.ToInt16(item.jobSeqNo),
                        jobDesc = item.jobDesc,
                        percentage = Convert.ToInt16(item.percentage),
                        jobDescSubList = subJobDesc.Where(e => e.jobDescCode == item.jobDescCode).OrderBy(o => o.subJDNo).ToList()
                    });
                }
                return finalJD;
            }
            catch (Exception)
            {
                return finalJD;
            }
        }


        [HttpPost]
        public JsonResult SaveJobDesc(tRSPPositionJobDesc data)
        {
            try
            {
                tRSPPositionJobDesc job = new tRSPPositionJobDesc();
                job.plantillaCode = data.plantillaCode;
                job.jobSeqNo = data.jobSeqNo;
                job.jobDesc = data.jobDesc;
                job.percentage = data.percentage;
                db.tRSPPositionJobDescs.Add(job);
                db.SaveChanges();
                List<PositionJobDesc> finalJD = _GetJobDescriptionList(data.plantillaCode);
                return Json(new { status = "success", list = finalJD }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult UpdateJobDesc(tRSPPositionJobDesc data)
        {
            try
            {
                tRSPPositionJobDesc job = db.tRSPPositionJobDescs.Single(e => e.recNo == data.recNo);
                job.jobSeqNo = data.jobSeqNo;
                job.jobDesc = data.jobDesc;
                job.percentage = data.percentage;
                db.SaveChanges();

                List<PositionJobDesc> finalJD = _GetJobDescriptionList(data.plantillaCode);
                return Json(new { status = "success", list = finalJD }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private int GenerateJobDescCode(IEnumerable<tRSPPositionJobDesc> jobDesc)
        {

            try
            {

                tRSPPositionJobDesc item = jobDesc.First();

                string code = "";
                code = "JD" + item.plantillaCode.Substring(4, item.plantillaCode.Length - 4) + DateTime.Now.ToString("YYMMddHHmmss");
                int counter = 0;

                foreach (tRSPPositionJobDesc job in jobDesc)
                {

                    counter = counter + 1;
                    string tmpCode = code + counter.ToString();

                    if (job.jobDescCode == null)
                    {

                    }

                }

                return 1;
            }
            catch (Exception)
            {

                return 0;
            }
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // PERSONNEL SERVICES



        public class PBOReportViewModel
        {
            public int itemNo { get; set; }
            public string EIC { get; set; }
            public string positionTitle { get; set; }
            public string nameOfIncumbent { get; set; }
            public int salaryGrade { get; set; }
            public int stepInc { get; set; }
            public string stepIncRemark { get; set; }
            public decimal monthlySalary { get; set; }
            public decimal annualSalary { get; set; }
            public decimal PERA { get; set; }
            public decimal RA { get; set; }
            public decimal TA { get; set; }
            public decimal clothing { get; set; }
            public decimal subsistence { get; set; }
            public decimal laundry { get; set; }
            public decimal hazardPay { get; set; }
            public decimal yearEndBonus { get; set; }
            public decimal cashGift { get; set; }
            public decimal loyaltyBonus { get; set; }
            public decimal midYearBonus { get; set; }
            public decimal lifeRetirement { get; set; }
            public decimal hdmf { get; set; }
            public decimal phic { get; set; }
            public decimal ecc { get; set; }
            public string remarks { get; set; }
        }


        //USE IN PROPOSED
        public IEnumerable<PBOReportViewModel> PersonnelServices(string deptCode, int propYear)
        {

            DateTime dt = DateTime.Now; // EFFECTIVE YEAR

            IEnumerable<vRSPPlantilla> p = db.vRSPPlantillas.Where(e => e.departmentCode == deptCode && e.itemNo > 0 && e.isFunded == true).OrderBy(o => o.itemNo).ToList();
            //IEnumerable<vRSPPlantilla> p = db.vRSPPlantillas.Where(e => e.functionCode == deptCode && e.itemNo > 0 && e.isFunded == true).OrderBy(o => o.itemNo).ToList();
            //SALARY CODE
            IEnumerable<tRSPSalaryTableDetail> sTable = db.tRSPSalaryTableDetails.Where(e => e.salaryCode == "MSSLT12021").ToList();

            //LOYALTY BONUS
            //IEnumerable<tRSPLoyalty> loyaltyList = HRIS.tRSPLoyalties.ToList();
            IEnumerable<tRSPLoyalty> loyaltyList = db.tRSPLoyalties.Where(e => e.loyalty > 1000000).ToList(); // MEANS NONE 

            List<PBOReportViewModel> myList = new List<PBOReportViewModel>();
            IEnumerable<tRSPNOSIPropBudYear> NOSIList = db.tRSPNOSIPropBudYears.Where(e => e.year == propYear).ToList();

            string midYearCutOff = "May 15, " + dt.Year;
            string yearEndCutOff = "October 31, " + dt.Year;

            foreach (vRSPPlantilla d in p)
            {

                string empName = d.fullNameLast;

                if (d.EIC == "SMPILATVBINOATO007197308090706")
                {
                    empName = d.fullNameLast;
                }


                int salaryGrade = Convert.ToInt16(d.salaryGrade);
                int step = Convert.ToInt16(d.step);
                if (d.step == 0 || d.step == null)
                { empName = "(VACANT)"; step = 1; }

                string remarks = "";
                decimal myPrevRate = newMonthlyRate(Convert.ToInt16(d.salaryGrade), step, sTable);
                decimal myNewRate = myPrevRate;

                decimal bonusMidYear = myPrevRate;
                decimal bonusYearEnd = myPrevRate;

                decimal hazardPay = 0;

                decimal phicPremGovt = Convert.ToDecimal(govtPremShare(Convert.ToDouble(myPrevRate)));

                decimal annualSalary = 0;
                decimal subsistence = 0;
                decimal laundry = 0;
                decimal gsisPrem = 0;

                //CHECK IF HAS STEP INCREMENT
                //HAS STEP
                string stepRemarks = "";
                decimal yearEndBonus = myNewRate;
                decimal totalStepIncrease = 0;

                //int hazardCode = GetHazardCode(d.idNo); 

                /////////// STEP INCREMENT
                tRSPNOSIPropBudYear hasStep = NOSIList.SingleOrDefault(e => e.EIC == d.EIC && e.year == propYear);

                int hazardCode = 0;


                // CHECK IF DEPT HAZARD
                // HAZARD CHECKER
                if (hazardCode == 0)
                {
                    if (deptCode == "OC191017163920527001") //PHO
                    {
                        hazardCode = 1;
                    }
                    else if (deptCode == "OC191017164319324001") //OC191017164319324001
                    {
                        hazardCode = 1;
                    }
                    else if (deptCode == "OC191017163949719001") //PSWDO
                    {
                        hazardCode = GetHazardCode(d.plantillaCode);
                    }
                }



                // NO HAZARD
                if (hazardCode == 0)
                {

                    // IF HAS STEP INCREMENT
                    if (hasStep != null)
                    {
                        DateTime stepEffectiveDate = Convert.ToDateTime(hasStep.effectiveDate);

                        int iMonth = stepEffectiveDate.Month;
                        int startMonth = (iMonth - 1);
                        int endMonth = 12 - startMonth;

                        step = Convert.ToInt16(hasStep.step); //APPLY NEW STEP INC
                        myNewRate = newMonthlyRate(Convert.ToInt16(d.salaryGrade), step, sTable);

                        string tmp = d.fullNameLast;

                        //CHECK MIDYEAR BONUS
                        midYearCutOff = "May 15, " + (dt.Year + 1);
                        if (stepEffectiveDate <= Convert.ToDateTime(midYearCutOff))
                        {
                            bonusMidYear = myNewRate;
                        }
                        else
                        {
                            bonusMidYear = myPrevRate;
                        }
                        //CHECK YEAR-END BONUS
                        yearEndCutOff = "October 31, " + (dt.Year + 1);
                        if (stepEffectiveDate <= Convert.ToDateTime(yearEndCutOff))
                        {
                            bonusYearEnd = myNewRate;
                        }
                        else
                        {
                            bonusYearEnd = myPrevRate;
                        }

                        stepRemarks = hasStep.remarks;
                        string[] words = stepRemarks.Split('-');
                        stepRemarks = words[0].ToString();

                        if (words.Count() > 1)
                        {
                            totalStepIncrease = Convert.ToDecimal(words[1]);
                        }
                        else
                        {
                            totalStepIncrease = 0;
                        }



                        // SALARY
                        annualSalary = myPrevRate * startMonth;
                        annualSalary = annualSalary + (myNewRate * endMonth);

                        // GSIS PREMIUM
                        decimal gsisPartA = (myPrevRate * Convert.ToDecimal(.12)) * startMonth;
                        decimal gsisPartB = (myNewRate * Convert.ToDecimal(.12)) * endMonth;
                        gsisPrem = gsisPartA + gsisPartB;

                        // PHIC CONTRIBUTION
                        double phicGovtShareA = CalQPHICGovtShare(Convert.ToDouble(myPrevRate), startMonth);
                        double phicGovtShareB = CalQPHICGovtShare(Convert.ToDouble(myNewRate), endMonth);
                        phicPremGovt = Convert.ToDecimal(phicGovtShareA + phicGovtShareB);

                    }
                    else
                    {
                        annualSalary = myPrevRate * 12;
                        bonusMidYear = myPrevRate;
                        bonusYearEnd = myPrevRate;
                        gsisPrem = (Convert.ToDecimal(myNewRate) * Convert.ToDecimal(.12)) * 12;
                        phicPremGovt = Convert.ToDecimal(CalQPHICGovtShare(Convert.ToDouble(myPrevRate), 12));
                    }
                    // *************************
                    // NO SUBS & LAUNDRY
                    subsistence = 0;
                    laundry = 0;

                }
                else if (hazardCode == 1) //HEALTH
                {

                    hasStep = NOSIList.SingleOrDefault(e => e.EIC == d.EIC);
                    // IF HAS  STEP INCREMENT THIS YEAR
                    if (hasStep != null)
                    {
                        // STEP EFFECTIVE DATE
                        // MAGNA CARTA DATE

                        DateTime stepEffectiveDate = Convert.ToDateTime(hasStep.effectiveDate);

                        if (d.EIC != null && dt.Year - d.birthDate.Value.Year == 65)
                        {
                            //MAGNA CARTA
                            DateTime magnaCartaDate = Convert.ToDateTime(d.birthDate);

                            if (stepEffectiveDate < magnaCartaDate)
                            {

                                int tmpCounter = 0;
                                tmpCounter = magnaCartaDate.Date.Month - 1;

                                int iMonth = stepEffectiveDate.Month;
                                int startMonth = (iMonth - 1);
                                int midMonth = tmpCounter - startMonth;
                                int endMonth = 12 - (startMonth - midMonth);

                                step = Convert.ToInt16(hasStep.step); //APPLY NEW STEP INC
                                myNewRate = newMonthlyRate(Convert.ToInt16(d.salaryGrade), step, sTable);

                                salaryGrade = Convert.ToInt16(d.salaryGrade) + 1;
                                decimal magnaCarRate = newMonthlyRate(salaryGrade, 1, sTable);



                                // IF THE SAME MONTH
                                if (magnaCartaDate.Date.Month == stepEffectiveDate.Month)
                                {
                                    startMonth = 0;
                                    midMonth = magnaCartaDate.Date.Month - 1;
                                    endMonth = 12 - midMonth;
                                }

                                ///////////////////////////////////////////////////////////////////////////////////////////////
                                // STEP INCREMENT
                                midYearCutOff = "May 15, " + hasStep.effectiveDate.Value.Year;
                                if (stepEffectiveDate <= Convert.ToDateTime(midYearCutOff))
                                {
                                    bonusMidYear = myNewRate;
                                }
                                else
                                {
                                    bonusMidYear = myPrevRate;
                                }

                                yearEndCutOff = "October 31, " + dt.Year;
                                if (stepEffectiveDate <= Convert.ToDateTime(yearEndCutOff))
                                {
                                    bonusYearEnd = myNewRate;
                                }
                                else
                                {
                                    bonusYearEnd = myPrevRate;
                                }


                                ///////////////////////////////////////////////////////////////////////////////////////////////
                                // STEP MAGNA CARTA
                                midYearCutOff = "May 15, " + hasStep.effectiveDate.Value.Year;
                                if (magnaCartaDate <= Convert.ToDateTime(midYearCutOff))
                                {
                                    bonusMidYear = magnaCarRate;
                                }
                                else
                                {
                                    if (stepEffectiveDate <= Convert.ToDateTime(midYearCutOff))
                                    {
                                        bonusMidYear = myNewRate;
                                    }
                                    else
                                    {
                                        bonusMidYear = myPrevRate;
                                    }
                                }
                                // YEAR IN MAGNA CARTA
                                yearEndCutOff = "October 31, " + dt.Year;
                                if (magnaCartaDate <= Convert.ToDateTime(yearEndCutOff))
                                {
                                    bonusYearEnd = magnaCarRate;
                                }
                                else
                                {
                                    if (stepEffectiveDate <= Convert.ToDateTime(midYearCutOff))
                                    {
                                        bonusYearEnd = myNewRate;
                                    }
                                    else
                                    {
                                        bonusYearEnd = myPrevRate;
                                    }
                                }


                            }
                            else
                            {

                            }

                        }
                        else
                        {

                            step = Convert.ToInt16(hasStep.step); //APPLY NEW STEP INC
                            myNewRate = newMonthlyRate(Convert.ToInt16(d.salaryGrade), step, sTable);

                            midYearCutOff = "May 15, " + hasStep.effectiveDate.Value.Year;
                            if (stepEffectiveDate <= Convert.ToDateTime(midYearCutOff))
                            {
                                bonusMidYear = myNewRate;
                            }
                            else
                            {
                                bonusMidYear = myPrevRate;
                            }

                            yearEndCutOff = "October 31, " + hasStep.effectiveDate.Value.Year;
                            if (stepEffectiveDate <= Convert.ToDateTime(yearEndCutOff))
                            {
                                bonusYearEnd = myNewRate;
                            }
                            else
                            {
                                bonusYearEnd = myPrevRate;
                            }

                            stepRemarks = hasStep.remarks;
                            string[] words = stepRemarks.Split('-');
                            stepRemarks = words[0].ToString();
                            totalStepIncrease = Convert.ToDecimal(words[1]);

                            int iMonth = stepEffectiveDate.Month;
                            int startMonth = (iMonth - 1);
                            int endMonth = 12 - startMonth;

                            // ANNUAL SALARY
                            annualSalary = myPrevRate * startMonth;
                            annualSalary = annualSalary + (myNewRate * endMonth);

                            //HAZARD
                            double partA = CalQHazadMonthly(salaryGrade, Convert.ToDouble(myPrevRate), startMonth);
                            double partB = CalQHazadMonthly(salaryGrade, Convert.ToDouble(myNewRate), endMonth);
                            hazardPay = Convert.ToDecimal(partA + partB);

                            // GSIS PREMIUM
                            decimal gsisPartA = (myPrevRate * Convert.ToDecimal(.12)) * startMonth;
                            decimal gsisPartB = (myNewRate * Convert.ToDecimal(.12)) * endMonth;
                            gsisPrem = gsisPartA + gsisPartB;
                            // PHIC CONTRIBUTION
                            double phicGovtShareA = CalQPHICGovtShare(Convert.ToDouble(myPrevRate), startMonth);
                            double phicGovtShareB = CalQPHICGovtShare(Convert.ToDouble(myNewRate), endMonth);
                            phicPremGovt = Convert.ToDecimal(phicGovtShareA + phicGovtShareB);

                        }

                    }
                    else //NO STEP INCREMENT
                    {
                        // ANNUAL SALARY
                        annualSalary = myPrevRate * 12;
                        // HAZARD
                        hazardPay = Convert.ToDecimal(CalQHazadMonthly(salaryGrade, Convert.ToDouble(myPrevRate), 12));
                        // MID YEAR
                        bonusMidYear = myPrevRate;
                        // YEAR END
                        bonusYearEnd = myPrevRate;
                        // GSIS GOVT PREM
                        gsisPrem = (Convert.ToDecimal(myPrevRate) * Convert.ToDecimal(.12)) * 12;
                        // PHIC
                        phicPremGovt = Convert.ToDecimal(CalQPHICGovtShare(Convert.ToDouble(myPrevRate), 12));
                    }

                    // SUBS AND  LAUNDRU
                    subsistence = 18000;
                    laundry = 1800;


                }
                else if (hazardCode == 2) //SOCIAL WORK
                {
                    hasStep = NOSIList.SingleOrDefault(e => e.EIC == d.EIC && e.year == 2021);
                    if (hasStep != null)
                    {
                        DateTime stepEffectiveDate = Convert.ToDateTime(hasStep.effectiveDate);

                        int iMonth = stepEffectiveDate.Month;
                        int startMonth = (iMonth - 1);
                        int endMonth = 12 - startMonth;

                        annualSalary = myPrevRate * startMonth;
                        annualSalary = annualSalary + (myNewRate * endMonth);
                        step = Convert.ToInt16(hasStep.step); //APPLY NEW STEP INC
                        myNewRate = newMonthlyRate(Convert.ToInt16(d.salaryGrade), step, sTable);

                        //CHECK MIDYEAR BONUS
                        midYearCutOff = "May 15, " + stepEffectiveDate.Year;
                        if (stepEffectiveDate <= Convert.ToDateTime(midYearCutOff))
                        {
                            bonusMidYear = myNewRate;
                        }
                        else
                        {
                            bonusMidYear = myPrevRate;
                        }
                        //CHECK YEAR-END BONUS
                        yearEndCutOff = "October 31, " + stepEffectiveDate.Year;
                        if (stepEffectiveDate <= Convert.ToDateTime(yearEndCutOff))
                        {
                            bonusYearEnd = myNewRate;
                        }
                        else
                        {
                            bonusYearEnd = myPrevRate;
                        }

                        //ANNUAL SALARY                           
                        annualSalary = myPrevRate * startMonth;
                        annualSalary = annualSalary + (myNewRate * endMonth);

                        ////HAZARD
                        //double partA = CalQHazadMonthly(salaryGrade, Convert.ToDouble(myPrevRate), startMonth);
                        //double partB = CalQHazadMonthly(salaryGrade, Convert.ToDouble(myNewRate), endMonth);

                        double partA = CalQlateHazardForSocialWorker(Convert.ToDouble(myPrevRate) * startMonth);
                        double partB = CalQlateHazardForSocialWorker(Convert.ToDouble(myNewRate) * endMonth);

                        hazardPay = Convert.ToDecimal(partA + partB);
                        //GSIS GOVT. CONTRIBUTION
                        // lifeRetirement = 
                        decimal gsisPartA = (myPrevRate * Convert.ToDecimal(.12)) * startMonth;
                        decimal gsisPartB = (myNewRate * Convert.ToDecimal(.12)) * endMonth;
                        gsisPrem = gsisPartA + gsisPartB;


                        // PHIC CONTRIBUTION
                        double phicGovtShareA = CalQPHICGovtShare(Convert.ToDouble(myPrevRate), startMonth);
                        double phicGovtShareB = CalQPHICGovtShare(Convert.ToDouble(myNewRate), endMonth);

                        phicPremGovt = Convert.ToDecimal(phicGovtShareA + phicGovtShareB);

                        stepRemarks = hasStep.remarks;
                        string[] words = stepRemarks.Split('-');
                        stepRemarks = words[0].ToString();
                        totalStepIncrease = Convert.ToDecimal(words[1]);

                    }
                    else
                    {
                        annualSalary = myPrevRate * 12;
                        bonusMidYear = myPrevRate;
                        bonusYearEnd = myPrevRate;
                        gsisPrem = (Convert.ToDecimal(myPrevRate) * Convert.ToDecimal(.12)) * 12;
                        phicPremGovt = Convert.ToDecimal(CalQPHICGovtShare(Convert.ToDouble(myPrevRate), 12));

                        //hazardPay = Convert.ToDecimal(CalQHazadMonthly(salaryGrade, Convert.ToDouble(myPrevRate), 12));

                        hazardPay = Convert.ToDecimal(CalQlateHazardForSocialWorker(Convert.ToDouble(myPrevRate))) * 12; //12 MONTHS

                        //DELETE FUNCTION
                        //hazardPay = Convert.ToDecimal(CalQlateHazardForSocialWorker(Convert.ToDouble(myNewRate)));
                    }

                    //SUBS
                    //PG HEAD. RAPISTA
                    //PG ASST.
                    //
                    subsistence = 0;
                    laundry = 0;

                    if (HasSubsistence(d.plantillaCode) == 1)
                    {
                        subsistence = 18000;
                    }


                }

                decimal RA = 0;
                decimal TA = 0;
                decimal loyalty = 0;
                // LOYALTY
                tRSPLoyalty loy = loyaltyList.SingleOrDefault(e => e.EIC == d.EIC);
                if (loy != null)
                {
                    loyalty = Convert.ToDecimal(loy.loyalty);
                }


                //RATA CHECKER & LOYALTY CHECKER
                tRSPRata rataData = getMyRATA(d.plantillaCode);
                if (rataData != null)
                {
                    RA = Convert.ToDecimal(rataData.RA) * 12;
                    TA = Convert.ToDecimal(rataData.TA) * 12;
                }

                if (d.EIC == "SMPILATVBINOATO007197308090706") //SPECIAL CODE FOR  (BM) MATOBATO
                {
                    //itm.annualSalary = 46023 * 12;
                    //itm.midYearBonus = 46023;
                    //itm.yearEndBonus = 46023;
                    //itm.RA = 1750 * 12;
                    myList.Add(new PBOReportViewModel()
                    {
                        EIC = d.EIC,
                        itemNo = Convert.ToInt16(d.itemNo),
                        positionTitle = d.shortPositionTitle,
                        nameOfIncumbent = empName,
                        salaryGrade = salaryGrade,
                        stepInc = 1,
                        stepIncRemark = "",
                        monthlySalary = 46023,
                        annualSalary = 46023 * 12, //(Convert.ToDecimal(myNewRate) * 12) + (totalStepIncrease),                       
                        PERA = 0,
                        RA = 1750 * 12,
                        TA = 0,
                        clothing = 0,
                        subsistence = 0,
                        laundry = 0,
                        hazardPay = 0,
                        cashGift = 0,
                        loyaltyBonus = 0,
                        midYearBonus = 46023, //Convert.ToDecimal(myNewRate),
                        yearEndBonus = 46023,
                        //lifeRetirement = (Convert.ToDecimal(myNewRate) * Convert.ToDecimal(.12)) * 12,
                        lifeRetirement = 0,
                        hdmf = 0,
                        phic = 0,
                        ecc = 0,
                        remarks = ""

                    });

                }

                else
                {
                    myList.Add(new PBOReportViewModel()
                    {
                        EIC = d.EIC,
                        itemNo = Convert.ToInt16(d.itemNo),
                        positionTitle = d.shortPositionTitle,
                        nameOfIncumbent = empName,
                        salaryGrade = salaryGrade,
                        stepInc = step,
                        stepIncRemark = stepRemarks,
                        monthlySalary = Convert.ToDecimal(myNewRate),
                        annualSalary = annualSalary, //(Convert.ToDecimal(myNewRate) * 12) + (totalStepIncrease),
                        PERA = 24000,
                        RA = RA,
                        TA = TA,
                        clothing = 6000,
                        subsistence = subsistence,
                        laundry = laundry,
                        hazardPay = hazardPay,
                        cashGift = 5000,
                        loyaltyBonus = loyalty,
                        midYearBonus = bonusMidYear, //Convert.ToDecimal(myNewRate),
                        yearEndBonus = bonusYearEnd,
                        lifeRetirement = (annualSalary) * Convert.ToDecimal(.12),
                        hdmf = 1200,
                        phic = phicPremGovt,
                        ecc = 1200,
                        remarks = remarks
                    });
                }
            }
            return myList;
        }


        public decimal newMonthlyRate(int salaryGrade, int step, IEnumerable<tRSPSalaryTableDetail> sTable)
        {
            decimal value = 0;
            tRSPSalaryTableDetail det = sTable.SingleOrDefault(e => e.salaryGrade == salaryGrade && e.step == step);
            if (det != null)
            {
                value = Convert.ToDecimal(det.rateMonth);
            }
            return value;
        }


        private int HasSubsistence(string pCode)
        {
            int res = 0;

            List<tRSPPlantilla> myList = new List<Models.tRSPPlantilla>();
            myList.Add(new tRSPPlantilla() { plantillaCode = "PC632027662" });  //PG HEAD
            myList.Add(new tRSPPlantilla() { plantillaCode = "PC1275890709" }); //PG ASST
            myList.Add(new tRSPPlantilla() { plantillaCode = "PC866569341" });  //SOCIAL WELFARE III
            myList.Add(new tRSPPlantilla() { plantillaCode = "PC77731620" });   //SOCIAL WELFARE III
            myList.Add(new tRSPPlantilla() { plantillaCode = "PC291612443" });   //SOCIAL WELFARE II
            myList.Add(new tRSPPlantilla() { plantillaCode = "PC403870139" });   //SOCIAL WELFARE I

            myList.Add(new tRSPPlantilla() { plantillaCode = "PC1566407906" });   //SOCIAL WELFARE II
            myList.Add(new tRSPPlantilla() { plantillaCode = "PC1070602817" });   //SOCIAL WELFARE I

            myList.Add(new tRSPPlantilla() { plantillaCode = "PC434303901" });   //SOCIAL WELFARE IV
            myList.Add(new tRSPPlantilla() { plantillaCode = "PC1532784360" });   //SOCIAL WELFARE I

            tRSPPlantilla tmp = myList.SingleOrDefault(e => e.plantillaCode == pCode);

            if (tmp != null)
            {
                res = 1;
            }

            return res;


        }

        private int GetHazardCode(string pCode)
        {
            int res = 2;

            List<tRSPPlantilla> myList = new List<Models.tRSPPlantilla>();
            myList.Add(new tRSPPlantilla() { plantillaCode = "PC1396523168" });  //NURSE II - CATOTAL
            myList.Add(new tRSPPlantilla() { plantillaCode = "PC2009198241" });  //NURSE I  - VACANT
            myList.Add(new tRSPPlantilla() { plantillaCode = "PC171282946" });   //SOCIAL WELFARE ASST -ABABON

            tRSPPlantilla tmp = myList.SingleOrDefault(e => e.plantillaCode == pCode);

            if (tmp != null)
            {
                res = 1;
            }

            return res;

        }

        private double govtPremShare(double rateMonth)
        {
            double monthlyPrem = 150;
            if (rateMonth >= 10000.01 && rateMonth <= 69999.99)
            {
                monthlyPrem = (rateMonth * .03) / 2;
            }
            else if (rateMonth >= 60000)
            {
                monthlyPrem = 1800 / 2;
            }
            return monthlyPrem * 12;
        }



        private tRSPRata getMyRATA(string plantillaCode)
        {
            List<tRSPRata> myList = new List<tRSPRata>();
            //GOV
            myList.Add(new tRSPRata() { EIC = "PC1395498714", RA = 11000, TA = 11000 });
            //VICE-GOV
            myList.Add(new tRSPRata() { EIC = "PC1002493235", RA = 10000, TA = 10000 });
            //SP
            myList.Add(new tRSPRata() { EIC = "PC1330822667", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC1376681184", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC1964721781", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC32882793", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC401484990", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC433938382", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC830271536", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC933216354", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC949228266", RA = 8500, TA = 8500 });
            //SP EX-O
            myList.Add(new tRSPRata() { EIC = "PC1024741735", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC447726653", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC680134735", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC896289307", RA = 8500, TA = 8500 });

            // PG DEPT HEAD
            myList.Add(new tRSPRata() { EIC = "PC2126664416", RA = 8500, TA = 0 });
            myList.Add(new tRSPRata() { EIC = "PC128392120", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC634339122", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC638378104", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC1400873911", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC108584726", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC1760980943", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC1141635432", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC1145456246", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC164905793", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC465459045", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC808265900", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC632027662", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC1769969647", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC2043679706", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC1105503071", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC341604431", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC1604107249", RA = 8500, TA = 8500 });
            myList.Add(new tRSPRata() { EIC = "PC772294349", RA = 8500, TA = 8500 });
            //CHIEF OF HOSPITALS
            myList.Add(new tRSPRata() { EIC = "PC1184815520", RA = 5000, TA = 5000 });
            myList.Add(new tRSPRata() { EIC = "PC1243518423", RA = 5000, TA = 5000 });
            myList.Add(new tRSPRata() { EIC = "PC2123177139", RA = 5000, TA = 5000 });
            myList.Add(new tRSPRata() { EIC = "PC552784905", RA = 5000, TA = 5000 });
            // ASST. PG DEPT.
            myList.Add(new tRSPRata() { EIC = "PC130320409", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC118754989", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC1194134706", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC1275890709", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC14917422", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC1663268962", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC167218590", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC1739288485", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC1947772203", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC1990933061", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC277201814", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC622132826", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC797488630", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC847159161", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC908815061", RA = 7500, TA = 7500 });
            myList.Add(new tRSPRata() { EIC = "PC654684200", RA = 7500, TA = 7500 });
            //myList.Add(new tRSPRata() { EIC = "PC654684200", RA = 7500, TA = 7500 });
            //PC797488630
            tRSPRata item = myList.SingleOrDefault(e => e.EIC == plantillaCode);
            return item;
        }

        private double CalQPHICGovtShare(double rateMonth, int monthCount)
        {
            double monthlyPrem = 400;
            if (rateMonth >= 10000.01 && rateMonth < 80000)
            {
                monthlyPrem = (rateMonth * .04) / 2;
            }
            else if (rateMonth >= 80000)
            {
                monthlyPrem = 3200 / 2;
            }
            return monthlyPrem * monthCount;
        }


        //HAZARD - Health
        private double CalQlateHazardForHealthPersonnel(int SG, double monthlyRate)
        {
            double total = 0;
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
            total = (monthlyRate * prcntg) * 12; // 12 months per year
            return total;
        }

        //HAZARD -  Social Worker
        private double CalQlateHazardForSocialWorker(double monthlyRate)
        {
            double dailyRate = (monthlyRate / 22) * .20;
            double hazard = (dailyRate * 30); // 12 MONTHS
            return hazard;
        }

        private double CalQHazadMonthly(int sg, double monthlyRate, int monthCount)
        {
            double total = 0; double prcntg = 0;
            if (sg <= 19)
            {
                prcntg = .25;
            }
            else if (sg == 20)
            {
                prcntg = .15;
            }
            else if (sg == 21)
            {
                prcntg = .13;
            }
            else if (sg == 22)
            {
                prcntg = .12;
            }
            else if (sg == 23)
            {
                prcntg = .11;
            }
            else if (sg == 24)
            {
                prcntg = .10;
            }
            else if (sg == 25)
            {
                prcntg = .10;
            }
            else if (sg == 26)
            {
                prcntg = .09;
            }
            else if (sg == 27)
            {
                prcntg = .08;
            }
            else if (sg == 28)
            {
                prcntg = .07;
            }
            else
            {
                prcntg = .05;
            }
            total = (monthlyRate * prcntg) * monthCount; //PER MONTH
            return total;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //PLANTILLA VACANT POSITION REPORT
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public JsonResult PrintPlantillaVacant(string id)
        {
            try
            {
                //int recCount = db.tRSPZPlantillaVacants.Where(e => e.printCode == id).Count();
                int recCount = 0;
                if (recCount == 0)
                {
                    IEnumerable<StructureParentViewModel> list = GeneratePlantillaVacantReport(id);
                }
                Session["ReportType"] = "PLANTILLAVACANT";
                Session["PrintReport"] = id;
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private IEnumerable<StructureParentViewModel> GeneratePlantillaVacantReport(string id)
        {

            tOrgFunction dept = db.tOrgFunctions.Single(e => e.functionCode == id);
            IEnumerable<vOrgStructure> struc = db.vOrgStructures.Where(e => e.departmentCode == dept.departmentCode).ToList();
            //IEnumerable<vRSPPlantillaProposed> deptPositionListPrinting = db.vRSPPlantillaProposeds.Where(e => e.functionCode == id && e.isFunded == false).OrderBy(o => o.plantillaNo).ToList();

            IEnumerable<vRSPPlantilla> deptPositionListPrinting = db.vRSPPlantillas.Where(e => e.functionCode == id && e.isFunded == false).OrderBy(o => o.plantillaNo).ToList();


            List<StructureParentViewModel> finList = new List<StructureParentViewModel>();
            List<vOrgStructure> structureChecker = new List<vOrgStructure>();
            string prevStructCode = "";
            int counter = 0;

            List<vRSPPlantillaProposed> printPreviewer = new List<vRSPPlantillaProposed>();
            foreach (vRSPPlantilla item in deptPositionListPrinting)
            {
                counter = counter + 1;
                string newHeaderCode = "";
                if (item.structureCode != prevStructCode)
                {
                    //CHECK IF SCTRUCTURE CODE EXIST
                    vOrgStructure checker = structureChecker.FirstOrDefault(e => e.structureCode == item.structureCode);

                    if (checker != null)
                    {
                        //ADD BLANK CHECKER
                        string tmpCode = "GROUP" + String.Format("{0:00000}", counter);
                        structureChecker.Add(new vOrgStructure()
                        {
                            structureCode = tmpCode,
                            //strucNo = item.strucNo
                        });
                        newHeaderCode = tmpCode;
                    }
                    else
                    {
                        //ADD NEW ITEM TO CHECKER
                        structureChecker.Add(new vOrgStructure()
                        {
                            structureCode = item.structureCode,
                            //strucNo = item.strucNo
                        });
                        newHeaderCode = item.structureCode;
                    }

                }
                else if (item.structureCode == prevStructCode)
                {
                    //ADD TO THE GROUP
                    newHeaderCode = item.structureCode;
                }

                printPreviewer.Add(new vRSPPlantillaProposed()
                {
                    plantillaCode = item.plantillaCode,
                    itemNo = item.itemNo,
                    oldItemNo = item.oldItemNo,
                    //rateCurrent = item.rateCurrent,
                    //rateProposed = item.rateProposed,
                    positionTitle = item.positionTitle,
                    subPositionTitle = item.subPositionTitle,
                    salaryGrade = item.salaryGrade,
                    step = item.step == 0 ? 1 : item.step,
                    positionLevel = item.positionLevel,
                    EIC = item.EIC,
                    fullNameLast = item.fullNameLast,
                    lastName = item.lastName,
                    firstName = item.firstName,
                    middleName = item.middleName,
                    extName = item.extName,
                    birthDate = item.birthDate,
                    isFunded = item.isFunded,
                    eligibilityName = item.eligibilityNameShort,
                    dateOrigAppointment = item.dateOrigAppointment,
                    dateLastPromoted = item.dateLastPromoted,
                    employmentStatus = item.employmentStatusNameShort,
                    //remarks = item.remarks,
                    departmentName = item.departmentName,
                    clusterCode = item.clusterCode,
                    clusterName = item.clusterName,
                    divisionCode = item.divisionCode,
                    divisionName = item.divisionName,
                    sectionCode = item.sectionCode,
                    sectionName = item.sectionName,
                    unitCode = item.unitCode,
                    unitName = item.unitName,
                    orderNo = item.plantillaNo,
                    //strucNo = item.strucNo,
                    structureCode = newHeaderCode
                });

                //SET NEW PREVIOUS CODE
                prevStructCode = item.structureCode;
            }

            var groupByPosition = printPreviewer.GroupBy(e => e.structureCode).Select(g => new { g.Key });


            foreach (var item in groupByPosition)
            {
                string s = item.Key;

                vOrgStructure nod = struc.SingleOrDefault(e => e.structureCode == s);

                string strucName = "";
                string parentPath = "";

                if (nod == null)
                {
                    nod = structureChecker.Single(e => e.structureCode == s);
                }
                else
                {
                    strucName = structName(nod);
                    parentPath = parentAddress(nod);
                }

                finList.Add(new StructureParentViewModel()
                {
                    structureCode = s,
                    structNo = Convert.ToInt16(nod.strucNo),
                    structureName = strucName,
                    structurePath = parentPath + "\\" + strucName,
                    parentPath = parentPath,
                    parentNo = Convert.ToInt16(nod.strucNo),
                    levelNo = 0,
                    orderNo = Convert.ToInt16(nod.orderNo),
                    positionList = PositionList2(s, printPreviewer, null).ToList()
                });
            }



            //ADD SAVING CODE FOR PRINTING

            //int rCount = db.tReportPlantillas.Where(e => e.functionCode == id).Count();
            int rCount = db.tRSPZPlantillaUnfunds.Where(e => e.functionCode == id).Count();

            if (rCount == 0)
            {
                //vOrgStructure t = structureChecker.FirstOrDefault(e => e.structureCode == item.structureCode);
                List<vOrgStructure> tempStructChecker = new List<vOrgStructure>();
                int iCount = 0;
                foreach (StructureParentViewModel item in finList)
                {
                    foreach (PlantillaPosition itm in item.positionList)
                    {
                        iCount = iCount + 1;
                        tRSPZPlantillaVacant rep = new tRSPZPlantillaVacant();

                        string clusterName = "";

                        int isStructClusExist = tempStructChecker.Where(e => e.structureCode == itm.clusterCode).Count();
                        if (isStructClusExist >= 1)
                        {
                            clusterName = null;
                        }
                        else
                        {
                            tempStructChecker.Add(new vOrgStructure()
                            {
                                structureCode = itm.clusterCode
                            });
                            clusterName = itm.clusterName != null ? itm.clusterName.ToUpper() : itm.clusterName;
                        }


                        string divName = "";
                        int isStructExist = tempStructChecker.Where(e => e.structureCode == itm.divisionCode).Count();
                        if (isStructExist >= 1)
                        {
                            divName = null;
                        }
                        else
                        {
                            tempStructChecker.Add(new vOrgStructure()
                            {
                                structureCode = itm.divisionCode
                            });
                            divName = itm.divisionName;
                        }

                        string sectionName = "";
                        int isStructExistSec = tempStructChecker.Where(e => e.structureCode == itm.sectionCode).Count();
                        if (isStructExistSec >= 1)
                        {
                            sectionName = null;
                        }
                        else
                        {
                            tempStructChecker.Add(new vOrgStructure()
                            {
                                structureCode = itm.sectionCode
                            });
                            sectionName = itm.sectionName;
                        }

                        string unitName = "";
                        int isStructExistUnit = tempStructChecker.Where(e => e.structureCode == itm.unitCode).Count();
                        if (isStructExistUnit >= 1)
                        {
                            unitName = null;
                        }
                        else
                        {
                            tempStructChecker.Add(new vOrgStructure()
                            {
                                structureCode = itm.unitCode
                            });
                            unitName = itm.unitName;
                        }



                        string tmpLevel = "";
                        if (itm.positionLevel == 1)
                        {
                            tmpLevel = "1st";
                        }
                        else if (itm.positionLevel == 2)
                        {
                            tmpLevel = "2nd";
                        }

                        //rep.functionCode = id;

                        rep.positionTitle = itm.positionTitle;
                        rep.subPositionTitle = itm.subPositionTitle;
                        rep.salaryGrade = itm.salaryGrade;
                        rep.positionLevel = tmpLevel;
                        rep.clusterCode = item.structureCode;
                        rep.clusterName = clusterName;
                        rep.divisionCode = itm.divisionCode;
                        rep.divisionName = divName;
                        rep.sectionCode = itm.sectionCode;
                        rep.sectionName = sectionName;
                        rep.unitCode = itm.unitCode;
                        rep.unitName = unitName;

                        rep.orderNo = iCount;
                        db.tRSPZPlantillaVacants.Add(rep);

                    }
                }
                db.SaveChanges();
            }

            return finList;

        }


        // *************************************************************************************************************************************************************************
        //  PS COMPUTATION
        // *************************************************************************************************************************************************************************

        public class TempMonthSavings
        {
            public string plantillaCode { get; set; }
            public string itemNo { get; set; }
            public string positionTitle { get; set; }
            public int salaryGrade { get; set; }
            public string monthlyRate { get; set; }
            public decimal totalSavings { get; set; }
        }

        public class TempPlantillaLog
        {
            public string plantillaCode { get; set; }
            public string itemNo { get; set; }
            public string logType { get; set; }
            public DateTime effectiveDate { get; set; }
        }


        [HttpPost]
        public JsonResult GenerateSavingsReport(string id, string code)
        {
            string plantillaCode = "";
            try
            {

                // !!! CHECK IF CODE HAS BEEN CREATED                
                // !!! CHECK FOR FUNDING STATUS
                DateTime dt = Convert.ToDateTime(id);
                DateTime dtMax = dt.AddMonths(1);

                int saveCounter = db.tRSPZPSMonthlySavings.Where(e => e.departmentCode == code && e.monthPeriod.Value.Year == dt.Year && e.monthPeriod.Value.Month == dt.Month).Count();
                int rowCounter = 0;

                //SEPARATION OF THE YEAR
                IEnumerable<vRSPSeparation> separationList = db.vRSPSeparations.Where(e => e.separationDate.Year == dt.Year).ToList();

                //APPOINTMENT OF THE YEAR
                IEnumerable<tRSPAppointment> appointmentList = db.tRSPAppointments.Where(e => e.effectivityDate.Value.Year == dt.Year).ToList();

                IEnumerable<vRSPPlantilla> list = db.vRSPPlantillas.Where(e => e.functionCode == code && e.isFunded == true).OrderBy(o => o.itemNo).ToList();

                IEnumerable<vRSPSalaryDetailCurrent> salaryTable = db.vRSPSalaryDetailCurrents.Where(e => e.isActive == true).ToList();

                List<TempMonthSavings> myList = new List<TempMonthSavings>();

                foreach (vRSPPlantilla item in list)
                {
                    int hasSavingsTag = 0;
                    plantillaCode = item.plantillaCode;



                    //PLANTILLA HAS APPOINTMENTLIST or SEPARATION LIST
                    IEnumerable<tRSPAppointment> appItem = appointmentList.Where(e => e.plantillaCode == item.plantillaCode).ToList();
                    IEnumerable<vRSPSeparation> sepItem = separationList.Where(e => e.plantillaCode == item.plantillaCode).ToList();

                    List<TempPlantillaLog> logs = new List<TempPlantillaLog>();

                    if (appItem.Count() >= 1)
                    {
                        foreach (tRSPAppointment appData in appItem)
                        {
                            logs.Add(new TempPlantillaLog()
                            {
                                plantillaCode = appData.plantillaCode,
                                itemNo = Convert.ToInt16(appData.itemNo).ToString("0000"),
                                logType = "APPT",
                                effectiveDate = Convert.ToDateTime(appData.effectivityDate)
                            });
                        }
                    }

                    if (sepItem.Count() >= 1)
                    {
                        foreach (vRSPSeparation sepData in sepItem)
                        {
                            logs.Add(new TempPlantillaLog()
                            {
                                plantillaCode = sepData.plantillaCode,
                                logType = "SEP",
                                effectiveDate = Convert.ToDateTime(sepData.separationDate)
                            });
                        }
                    }

                    if (item.EIC == null)
                    {
                        tRSPEmployeePosition prevData = db.tRSPEmployeePositions.Where(e => e.plantillaCode == item.plantillaCode).OrderByDescending(o => o.recNo).FirstOrDefault();
                        if (prevData != null)
                        {
                            tRSPEmployeePosition newPosition = db.tRSPEmployeePositions.Where(e => e.EIC == prevData.EIC).OrderByDescending(o => o.recNo).FirstOrDefault();
                            if (newPosition.effectiveDate.Value.Year == dt.Year)
                            {
                                logs.Add(new TempPlantillaLog()
                                {
                                    plantillaCode = item.plantillaCode,
                                    logType = "SEP",
                                    effectiveDate = Convert.ToDateTime(newPosition.effectiveDate.Value.AddDays(-1))
                                });
                            }
                        }
                    }



                    //ORDER BY DATE
                    logs = logs.OrderBy(o => o.effectiveDate).ToList();

                    if (item.EIC != null)       //OCCUPIED
                    {
                        //IF HAS SEPARATION OR APPOINTMENT FOR THIS YEAR                      
                        if (logs.Count() == 0)
                        {  //NO SAVINGS
                            hasSavingsTag = 0;
                        }
                        else if (logs.Count() >= 1)
                        {
                            hasSavingsTag = HasSavingOnThisMonth(logs, dt, item.EIC);
                        }
                    }
                    else if (item.EIC == null)  //VACANT
                    {

                        if (logs.Count() == 0)
                        {
                            //HAS SAVINGS
                            hasSavingsTag = 1;
                        }
                        else if (logs.Count() >= 1)
                        {
                            hasSavingsTag = HasSavingOnThisMonth(logs, dt, item.EIC);
                        }
                    }



                    //tRSPSeparation sepItem = separationList.SingleOrDefault(e => e.effectiveDate.Month == dt.Month);
                    //tRSPAppointment appItem = appointmentList.SingleOrDefault(e => e.effectivityDate.Value.Month == dt.Month);
                    vRSPSalaryDetailCurrent salary = salaryTable.Single(e => e.salaryGrade == item.salaryGrade);
                    decimal tempRate = Convert.ToDecimal(salary.rateMonth);

                    if (item.EIC == null) //IF VACANT and no separation for this month
                    {
                        //if (item.itemNo == 171)
                        //{
                        //    hasSavingsTag = 0;
                        //}
                        //DateTime tmpDT = dt.AddMonths(1).AddDays(-1);

                        ////check if has no appointment for this month
                        //tRSPAppointment appItem = appointmentList.SingleOrDefault(e => e.plantillaCode == item.plantillaCode && e.effectivityDate >= dt && e.effectivityDate < dtMax);

                        ////GET LAST EMPLOYEE EIC IF NOT PROMOTED THIS MONTH
                        //int isPromotedTag = 0;
                        //tRSPEmployeePosition prevData = db.tRSPEmployeePositions.Where(e => e.plantillaCode == item.plantillaCode).OrderByDescending(o => o.recNo).FirstOrDefault(e => e.plantillaCode == item.plantillaCode);
                        //if (prevData == null)
                        //{
                        //    isPromotedTag = 0;
                        //}
                        //else
                        //{
                        //    //get effective date if belong to this month
                        //    tRSPEmployeePosition newPosition = db.tRSPEmployeePositions.Where(e => e.EIC == prevData.EIC).OrderByDescending(o => o.recNo).FirstOrDefault();
                        //    if (newPosition.effectiveDate >= dt || newPosition.effectiveDate < dtMax)
                        //    {
                        //        isPromotedTag = 1;
                        //    }                             
                        //} 

                        //vRSPSeparation sepItem = separationList.SingleOrDefault(e => e.plantillaCode == item.plantillaCode);
                        //if (appItem == null && sepItem == null && isPromotedTag == 0)
                        //{
                        //    hasSavingsTag = 1;
                        //}
                    }
                    else if (item != null)
                    {
                        //tRSPAppointment appItem = appointmentList.SingleOrDefault(e => e.plantillaCode == item.plantillaCode);
                        //if (appItem != null)
                        //{
                        //    DateTime tmpDT = dtMax.AddMonths(1).AddDays(-1);
                        //    if (appItem.effectivityDate > tmpDT)
                        //    {
                        //        hasSavingsTag = 1;
                        //    }
                        //}
                    }

                    if (hasSavingsTag == 1)
                    {
                        //if (dt.Year == 2021 && dt.Month == 3)
                        //{
                        //    int hasMarchPending = CheckPendingAppointment(Convert.ToInt16(item.itemNo));
                        //    if (hasMarchPending >= 1)
                        //    {
                        //        hasSavingsTag = 0;
                        //    }
                        //} 
                    }

                    if (hasSavingsTag == 1)
                    {
                        //add to savings list                                              
                        string printCode = dt.ToString("yyyyMM") + code;
                        myList.Add(new TempMonthSavings()
                        {
                            plantillaCode = item.plantillaCode,
                            itemNo = Convert.ToInt16(item.newItemNo).ToString("0000"),
                            monthlyRate = Convert.ToDecimal(tempRate).ToString("#,##0.00"),
                            positionTitle = item.positionTitle,
                            salaryGrade = Convert.ToInt16(item.salaryGrade)
                        });

                        if (saveCounter == 0)
                        {
                            tRSPZPSMonthlySaving temp = new tRSPZPSMonthlySaving();
                            temp = _GetPositionSavings(item, tempRate);
                            temp.monthPeriod = dt;
                            temp.departmentCode = code;
                            temp.monthCode = printCode;
                            db.tRSPZPSMonthlySavings.Add(temp);
                            rowCounter = rowCounter + 1;
                        }
                    }
                }

                if (rowCounter >= 1)
                {
                    db.SaveChanges();
                }



                return Json(new { status = "success", list = myList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string tmp = plantillaCode;
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        public int HasSavingOnThisMonth(List<TempPlantillaLog> logs, DateTime dt, string EIC)
        {
            try
            {

                int res = 0;


                int currentTag = 0; //0 VACANT 1; OCCUPIED
                int idxMonth = 1;

                int[] myMonth = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                //firstLog
                TempPlantillaLog firstLog = logs.First();

                if (firstLog.plantillaCode == "PC548054380")
                {
                    string tmp = "";
                }

                TempPlantillaLog nextLog = new TempPlantillaLog();
                if (logs.Count >= 2)
                {
                    nextLog = logs[1];
                }

                string myTag = "APPT";
                if (firstLog.logType == "APPT")
                {
                    myTag = "APPT";
                    idxMonth = firstLog.effectiveDate.Month;
                    for (int i = 1; i <= 12; i++)
                    {
                        if (myTag == "APPT")
                        {
                            if (i < idxMonth)
                            {
                                myMonth[i] = 1;
                            }
                            else
                            {
                                myMonth[i] = 0;
                            }
                        }
                        else if (myTag == "SEP")
                        {
                            if (idxMonth < i)
                            {
                                myMonth[i] = 0;
                            }
                            else
                            {
                                myMonth[i] = 1;
                            }
                        }

                        if (nextLog != null && i < 12)
                        {
                            if (nextLog.effectiveDate.Month == i + 1)
                            {
                                myTag = nextLog.logType;
                                idxMonth = nextLog.effectiveDate.Month;
                                currentTag = currentTag + 1;

                                if (logs.Count() > currentTag + 1)
                                {
                                    nextLog = logs[currentTag + 1];
                                }
                                else
                                {
                                    nextLog = null;
                                }
                            }
                        }

                    }

                    res = myMonth[dt.Month];

                }
                else
                {
                    myTag = "SEP";
                    idxMonth = firstLog.effectiveDate.Month;

                    for (int i = 1; i <= 12; i++)
                    {
                        if (myTag == "SEP")
                        {
                            if (i <= idxMonth)
                            {
                                myMonth[i] = 0;
                            }
                            else
                            {
                                myMonth[i] = 1;
                            }

                        }
                        else if (myTag == "APPT")
                        {
                            if (idxMonth <= i)
                            {
                                myMonth[i] = 0;
                            }
                            else
                            {
                                myMonth[i] = 1;
                            }
                        }


                        if (nextLog != null && i < 12)
                        {
                            if (nextLog.effectiveDate.Month == i + 1)
                            {
                                myTag = nextLog.logType;
                                idxMonth = nextLog.effectiveDate.Month;
                                currentTag = currentTag + 1;

                                if (logs.Count() > currentTag + 1)
                                {
                                    nextLog = logs[currentTag + 1];
                                }
                                else
                                {
                                    nextLog = null;
                                }

                            }
                        }

                    }

                    res = myMonth[dt.Month];

                }





                return res;

            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                return -1;
            }
        }


        public class TempPendingAppointment
        {
            public int itemNo { get; set; }
            public DateTime appointmentDate { get; set; }
        }

        private int CheckPendingAppointment(int itemNo)
        {
            // HAS MARCH PENDING
            int hasPending = 0;
            List<TempPendingAppointment> myList = new List<TempPendingAppointment>();
            myList.Add(new TempPendingAppointment() { itemNo = 762, appointmentDate = Convert.ToDateTime("March 1, 2021") });
            myList.Add(new TempPendingAppointment() { itemNo = 795, appointmentDate = Convert.ToDateTime("March 1, 2021") });
            myList.Add(new TempPendingAppointment() { itemNo = 745, appointmentDate = Convert.ToDateTime("March 1, 2021") });
            myList.Add(new TempPendingAppointment() { itemNo = 744, appointmentDate = Convert.ToDateTime("March 1, 2021") });
            myList.Add(new TempPendingAppointment() { itemNo = 748, appointmentDate = Convert.ToDateTime("March 1, 2021") });
            myList.Add(new TempPendingAppointment() { itemNo = 797, appointmentDate = Convert.ToDateTime("March 1, 2021") });

            //SAO - PGSO INVENTORY
            myList.Add(new TempPendingAppointment() { itemNo = 254, appointmentDate = Convert.ToDateTime("March 1, 2021") });
            //myList.Add(new TempPendingAppointment() { itemNo = 222, appointmentDate = Convert.ToDateTime("March 1, 2021") });

            //PEO
            myList.Add(new TempPendingAppointment() { itemNo = 650, appointmentDate = Convert.ToDateTime("March 1, 2021") });
            myList.Add(new TempPendingAppointment() { itemNo = 736, appointmentDate = Convert.ToDateTime("March 1, 2021") });
            myList.Add(new TempPendingAppointment() { itemNo = 662, appointmentDate = Convert.ToDateTime("March 1, 2021") });

            hasPending = myList.Where(e => e.itemNo == itemNo).Count();
            return hasPending;
        }


        private tRSPZPSMonthlySaving _GetPositionSavings(vRSPPlantilla p, decimal rateMonth)
        {

            //p.rateMonth = rateMonth;

            double gsisPrem = (Convert.ToDouble(rateMonth) * Convert.ToDouble(.12)) * 1;
            double phicPrem = CalQPHICGovtShare(Convert.ToDouble(rateMonth), 1);

            tRSPRata rataData = getMyRATA(p.plantillaCode);
            decimal ra = 0;
            decimal ta = 0;
            if (rataData != null)
            {
                ra = Convert.ToDecimal(rataData.RA);
                ta = Convert.ToDecimal(rataData.TA);
            }

            int hazardCode = 0;

            // CHECK IF DEPT HAZARD
            // HAZARD CHECKER
            if (hazardCode == 0)
            {
                if (p.departmentCode == "OC191017163920527001") //PHO
                {
                    hazardCode = 1;
                }
                else if (p.departmentCode == "OC191017164319324001") //OC191017164319324001
                {
                    hazardCode = 1;
                }
                else if (p.departmentCode == "OC191017163949719001") //PSWDO
                {
                    hazardCode = GetHazardCode(p.plantillaCode);
                }
            }

            decimal ECC = 100;
            decimal hdmfPrem = 100;
            decimal PERA = 2000;

            double hazard = 0;
            decimal laundry = 0;
            decimal subs = 0;

            if (hazardCode >= 1 && hazardCode <= 2)
            {
                hazard = CalQHazadMonthly(Convert.ToInt16(p.salaryGrade), Convert.ToDouble(rateMonth), 1);
                if (hazardCode == 1)
                {
                    subs = 1500;
                    laundry = 150;
                }
                else if (hazardCode == 2)
                {
                    subs = 1500;
                    laundry = 0;
                }
            }


            decimal totSavings = Convert.ToDecimal(rateMonth) + PERA + ra + ta + Convert.ToDecimal(hazard) + subs + laundry + Convert.ToDecimal(gsisPrem) + ECC + hdmfPrem + Convert.ToDecimal(phicPrem);

            tRSPZPSMonthlySaving res = new tRSPZPSMonthlySaving();
            res.plantillaCode = p.plantillaCode;
            res.monthlyRate = rateMonth;
            res.PERA = PERA;
            res.RA = ra;
            res.TA = ta;
            res.hazard = Convert.ToDecimal(hazard);
            res.laundry = laundry;
            res.subsistence = subs;
            res.gsisPrem = Convert.ToDecimal(gsisPrem);
            res.ECC = ECC;
            res.phicPrem = Convert.ToDecimal(phicPrem);
            res.hdmfPrem = hdmfPrem;
            res.totalSavings = totSavings;

            //decimal lifeAndRetire = (Convert.ToDecimal(monthlyRate) * Convert.ToDecimal(.12)) * monthCount;
            //decimal PHIC = CalQPHICGovtShare(Convert.ToDouble(monthlyRate), monthCount);

            return res;



        }


        //

        [HttpPost]
        public JsonResult GetLastItemNo()
        {
            try
            {
                tRSPPlantilla p = db.tRSPPlantillas.Where(e => e.isFunded == true).OrderByDescending(o => o.newItemNo).First();
                string newItemNo = Convert.ToInt16(p.newItemNo + 1).ToString("0000");

                return Json(new { status = "success", newItemNo = newItemNo }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult FundPositionNextYear(tRSPPlantilla data)
        {
            try
            {
                tRSPPlantilla p = db.tRSPPlantillas.Single(e => e.plantillaCode == data.plantillaCode);
                p.fundStat = 2;
                db.SaveChanges();
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult FundPosition(tRSPPlantilla data)
        {
            try
            {
                tRSPPlantilla p = db.tRSPPlantillas.SingleOrDefault(e => e.plantillaCode == data.plantillaCode && e.isFunded == false);

                if (p == null)
                {
                    return Json(new { status = "Invalid operation!" }, JsonRequestBehavior.AllowGet);
                }

                tRSPPlantilla r = db.tRSPPlantillas.Where(e => e.isFunded == true).OrderByDescending(o => o.newItemNo).First();
                int newItemNo = Convert.ToInt16(r.newItemNo + 1);

                p.isFunded = true;
                p.newItemNo = newItemNo;
                db.SaveChanges();

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }



        //REPORT ////////////////////////////////////////////////////////////////
        
        //PLANTILLA: UNFUNDED CURRENT
        [HttpPost]
        public JsonResult ShowUnfundedPositions(string id)
        {
            try
            {
                //BY FUNCTION CODE
                IEnumerable<StructureParentViewModel> list = DeptCurrentStructure(id, -1);
                //Session["PlantillaCurrent"] = list;
                Session["PlantillaUnfunded"] = list;
                return Json(new { status = "success", plantilla = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string errMsg = ex.ToString();
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
            }
        }


        //NG
        [HttpPost]
        public JsonResult PrintPlantillaUnfundSetup(string id)
        {

            int recCount = db.tRSPZPlantillaUnfunds.Where(e => e.functionCode == id && e.budgetYear == 2022).Count();
            if (recCount == 0)
            {
                IEnumerable<StructureParentViewModel> list = DeptProposedStructureUnfunded(id);
            }

            //tOrgFunction deptFunc = db.tOrgFunctions.Single(e => e.functionCode == id);
            Session["ReportType"] = "PlantillaUnfunded";
            Session["PrintReport"] = id;

            return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
        }


        private IEnumerable<StructureParentViewModel> DeptProposedStructureUnfunded(string id)
        {

            tOrgFunction dept = db.tOrgFunctions.Single(e => e.functionCode == id);
            IEnumerable<vOrgStructure> struc = db.vOrgStructures.Where(e => e.departmentCode == dept.departmentCode).ToList();
            //IEnumerable<vRSPPlantillaProposed> deptPositionListPrinting = db.vRSPPlantillaProposeds.Where(e => e.functionCode == id && e.isFunded == false).OrderBy(o => o.plantillaNo).ToList();

            IEnumerable<vRSPPlantilla> deptPositionListPrinting = db.vRSPPlantillas.Where(e => e.functionCode == id && e.isFunded == false && e.fundStat == 0).OrderBy(o => o.plantillaNo).ToList();
            
            List<StructureParentViewModel> finList = new List<StructureParentViewModel>();
            List<vOrgStructure> structureChecker = new List<vOrgStructure>();
            string prevStructCode = "";
            int counter = 0;

            List<vRSPPlantillaProposed> printPreviewer = new List<vRSPPlantillaProposed>();
            foreach (vRSPPlantilla item in deptPositionListPrinting)
            {
                counter = counter + 1;
                string newHeaderCode = "";
                if (item.structureCode != prevStructCode)
                {
                    //CHECK IF SCTRUCTURE CODE EXIST
                    vOrgStructure checker = structureChecker.FirstOrDefault(e => e.structureCode == item.structureCode);

                    if (checker != null)
                    {
                        //ADD BLANK CHECKER
                        string tmpCode = "GROUP" + String.Format("{0:00000}", counter);
                        structureChecker.Add(new vOrgStructure()
                        {
                            structureCode = tmpCode,
                            //strucNo = item.strucNo
                        });
                        newHeaderCode = tmpCode;
                    }
                    else
                    {
                        //ADD NEW ITEM TO CHECKER
                        structureChecker.Add(new vOrgStructure()
                        {
                            structureCode = item.structureCode,
                            //strucNo = item.strucNo
                        });
                        newHeaderCode = item.structureCode;
                    }

                }
                else if (item.structureCode == prevStructCode)
                {
                    //ADD TO THE GROUP
                    newHeaderCode = item.structureCode;
                }

                printPreviewer.Add(new vRSPPlantillaProposed()
                {
                    plantillaCode = item.plantillaCode,
                    itemNo = item.itemNo,
                    oldItemNo = item.oldItemNo,
                    //rateCurrent = item.rateCurrent,
                    //rateProposed = item.rateProposed,
                    positionTitle = item.positionTitle,
                    subPositionTitle = item.subPositionTitle,
                    salaryGrade = item.salaryGrade,
                    step = item.step == 0 ? 1 : item.step,
                    positionLevel = item.positionLevel,
                    EIC = item.EIC,
                    fullNameLast = item.fullNameLast,
                    lastName = item.lastName,
                    firstName = item.firstName,
                    middleName = item.middleName,
                    extName = item.extName,
                    birthDate = item.birthDate,
                    isFunded = item.isFunded,
                    eligibilityName = item.eligibilityNameShort,
                    dateOrigAppointment = item.dateOrigAppointment,
                    dateLastPromoted = item.dateLastPromoted,
                    employmentStatus = item.employmentStatusNameShort,
                    //remarks = item.remarks,
                    departmentName = item.departmentName,
                    clusterCode = item.clusterCode,
                    clusterName = item.clusterName,
                    divisionCode = item.divisionCode,
                    divisionName = item.divisionName,
                    sectionCode = item.sectionCode,
                    sectionName = item.sectionName,
                    unitCode = item.unitCode,
                    unitName = item.unitName,
                    orderNo = item.plantillaNo,
                    //strucNo = item.strucNo,
                    structureCode = newHeaderCode
                });

                //SET NEW PREVIOUS CODE
                prevStructCode = item.structureCode;
            }

            var groupByPosition = printPreviewer.GroupBy(e => e.structureCode).Select(g => new { g.Key });


            foreach (var item in groupByPosition)
            {
                string s = item.Key;

                vOrgStructure nod = struc.SingleOrDefault(e => e.structureCode == s);

                string strucName = "";
                string parentPath = "";

                if (nod == null)
                {
                    nod = structureChecker.Single(e => e.structureCode == s);
                }
                else
                {
                    strucName = structName(nod);
                    parentPath = parentAddress(nod);
                }

                finList.Add(new StructureParentViewModel()
                {
                    structureCode = s,
                    structNo = Convert.ToInt16(nod.strucNo),
                    structureName = strucName,
                    structurePath = parentPath + "\\" + strucName,
                    parentPath = parentPath,
                    parentNo = Convert.ToInt16(nod.strucNo),
                    levelNo = 0,
                    orderNo = Convert.ToInt16(nod.orderNo),
                    positionList = PositionList2(s, printPreviewer, null).ToList()
                });
            }



            //ADD SAVING CODE FOR PRINTING

            //int rCount = db.tReportPlantillas.Where(e => e.functionCode == id).Count();
            int rCount = db.tRSPZPlantillaUnfunds.Where(e => e.functionCode == id && e.budgetYear == 2022).Count();

            if (rCount == 0)
            {
                //vOrgStructure t = structureChecker.FirstOrDefault(e => e.structureCode == item.structureCode);
                List<vOrgStructure> tempStructChecker = new List<vOrgStructure>();
                int iCount = 0;
                foreach (StructureParentViewModel item in finList)
                {
                    foreach (PlantillaPosition itm in item.positionList)
                    {

                        iCount = iCount + 1;
                        //tReportPlantilla rep = new tReportPlantilla();
                        tRSPZPlantillaUnfund rep = new tRSPZPlantillaUnfund();

                        string clusterName = "";

                        int isStructClusExist = tempStructChecker.Where(e => e.structureCode == itm.clusterCode).Count();
                        if (isStructClusExist >= 1)
                        {
                            clusterName = null;
                        }
                        else
                        {
                            tempStructChecker.Add(new vOrgStructure()
                            {
                                structureCode = itm.clusterCode
                            });
                            clusterName = itm.clusterName != null ? itm.clusterName.ToUpper() : itm.clusterName;
                        }


                        string divName = "";
                        int isStructExist = tempStructChecker.Where(e => e.structureCode == itm.divisionCode).Count();
                        if (isStructExist >= 1)
                        {
                            divName = null;
                        }
                        else
                        {
                            tempStructChecker.Add(new vOrgStructure()
                            {
                                structureCode = itm.divisionCode
                            });
                            divName = itm.divisionName;
                        }

                        string sectionName = "";
                        int isStructExistSec = tempStructChecker.Where(e => e.structureCode == itm.sectionCode).Count();
                        if (isStructExistSec >= 1)
                        {
                            sectionName = null;
                        }
                        else
                        {
                            tempStructChecker.Add(new vOrgStructure()
                            {
                                structureCode = itm.sectionCode
                            });
                            sectionName = itm.sectionName;
                        }

                        string unitName = "";
                        int isStructExistUnit = tempStructChecker.Where(e => e.structureCode == itm.unitCode).Count();
                        if (isStructExistUnit >= 1)
                        {
                            unitName = null;
                        }
                        else
                        {
                            tempStructChecker.Add(new vOrgStructure()
                            {
                                structureCode = itm.unitCode
                            });
                            unitName = itm.unitName;
                        }



                        string tmpLevel = "";
                        if (itm.positionLevel == 1)
                        {
                            tmpLevel = "1st";
                        }
                        else if (itm.positionLevel == 2)
                        {
                            tmpLevel = "2nd";
                        }

                        rep.functionCode = id;
                        rep.budgetYear = 2022;
                        //rep.oldItemNo = Convert.ToInt16(itm.oldItemNo);
                        //rep.itemNo = Convert.ToInt16(itm.itemNo);
                        //rep.currentYearRate = Convert.ToDecimal(itm.currentYearRate);
                        //rep.proposedYearRate = itm.proposeYearRate == 0 ? Convert.ToDecimal(itm.currentYearRate) : Convert.ToDecimal(itm.proposeYearRate);
                        rep.positionTitle = itm.positionTitle;
                        rep.subPositionTitle = itm.subPositionTitle;
                        rep.salaryGrade = itm.salaryGrade;
                        //rep.step = itm.step == 0 ? 1 : itm.step;

                        rep.positionLevel = tmpLevel;
                        //rep.lastName = itm.lastName;
                        //rep.firstName = itm.firstName;
                        //rep.middleName = itm.middleName;
                        //rep.extnName = itm.extName;

                        //rep.birthDate = itm.birthDate;
                        //rep.eligibility = itm.eligibilityName;
                        //rep.dateOrigAppointment = itm.origApptDate;
                        //rep.dateLastPromoted = itm.lastPromDate;
                        //rep.statusName = itm.empStatus;
                        //rep.remarks = itm.EIC == null ? "VACANT" : itm.remarks;

                        rep.clusterCode = item.structureCode;
                        rep.clusterName = clusterName;

                        rep.divisionCode = itm.divisionCode;
                        rep.divisionName = divName;
                        rep.sectionCode = itm.sectionCode;
                        rep.sectionName = sectionName;
                        rep.unitCode = itm.unitCode;
                        rep.unitName = unitName;

                        rep.orderNo = iCount;
                        db.tRSPZPlantillaUnfunds.Add(rep);

                    }
                }
                db.SaveChanges();
            }

            return finList;

        }

        ///////////////////////////////////////
    }
}