app.controller('WorkGroupApp', ['$scope', '$http', '$document', function (s, h, d) {
     


    
    
    s.data = {};
    s.workGroup = {};

    s.departmentList = [
         { departmentCode: "OC191015134332380001", shortDepartmentName: 'PGO' },
         { departmentCode: "OC191015134453048001", shortDepartmentName: 'PADO' },
         { departmentCode: "OC191015134636261001", shortDepartmentName: 'PHRMO' },
         { departmentCode: "OC191015134702339001", shortDepartmentName: 'PICKMO' },
         { departmentCode: "OC191015152447314001", shortDepartmentName: 'PPDO' },
         { departmentCode: "OC191015152503876001", shortDepartmentName: 'PGSO' },
         { departmentCode: "OC191015152544060001", shortDepartmentName: 'PBO' },
         { departmentCode: "OC191015152600833001", shortDepartmentName: 'PACCO' },
         { departmentCode: "OC191017163038011001", shortDepartmentName: 'PLO' },
         { departmentCode: "OC191017163348136001", shortDepartmentName: 'PTO' },
         { departmentCode: "OC191017163416304001", shortDepartmentName: 'PASSO' },
         { departmentCode: "OC191017163920527001", shortDepartmentName: 'PHO' },
         { departmentCode: "OC191017163949719001", shortDepartmentName: 'PSWDO' },
         { departmentCode: "OC191017164019805001", shortDepartmentName: 'PAGRO' },
         { departmentCode: "OC191017164040738001", shortDepartmentName: 'PVO' },
         { departmentCode: "OC191017164103801001", shortDepartmentName: 'PENRO' },
         { departmentCode: "OC191017164206284001", shortDepartmentName: 'PEO' },
         { departmentCode: "OC191017164319324001", shortDepartmentName: 'PEEDO' },
         { departmentCode: "OC191017164344755001", shortDepartmentName: 'PSYDO' },
         { departmentCode: "OC191017164437082001", shortDepartmentName: 'VGO' },
         { departmentCode: "OC191017164422254001", shortDepartmentName: 'SPO' },
         { departmentCode: "OC191017164406966001", shortDepartmentName: 'OSS' }
    ];
    
    s.emptStatus = [{ employmentStatus: 'CASUAL', employmentStatusCode: '05' }, { employmentStatus: 'JOB ORDER', employmentStatusCode: '06' }, { employmentStatus: 'CONTRACT OF SERVICE', employmentStatusCode: '07' }, { employmentStatus: 'HONORARIUM', employmentStatusCode: '08' }];
    
    loadWorkGroupInitData();

   function loadWorkGroupInitData() {
       h.post('~/../../RSPUtility/WorkGroupInit').then(function (r) {
           if (r.data.status == "success") {
               s.workGroup = r.data.workGroup;
           }
       });
    }

    s.loadEmpListByWorkGroup = function (data) {       
        if (data.workGroupCode != undefined && data.employmentStatusCode) {           
            h.post('~/../../RSPUtility/GetWorkGroupEmployee', { code: data.workGroupCode, id: data.employmentStatusCode }).then(function (r) {
                if (r.data.status == "success") {
                    s.employee = r.data.employee;
                }
            });
        }
    }

    s.printPreview = function (data) {
        
        if (data.workGroupCode != undefined && data.employmentStatusCode) {
            window.open("../Reports/HRIS.aspx");
        }

    }
     
}]);


