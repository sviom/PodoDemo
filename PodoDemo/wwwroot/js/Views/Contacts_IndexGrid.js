// 기본 출력 항목 표시
function SetContactsIndex(contactListRaw) {
    var source =
        {
            localdata: contactListRaw,
            datatype: "json",
            datafields:
            [
                { name: 'Contactid', type: 'number' },
                { name: 'Name', type: 'string' },
                { name: 'Department', type: 'string' },
                { name: 'Title', type: 'string' },
                { name: 'Account', type: 'string', map: 'Account>Name' },
                { name: 'Email', type: 'string' },
                { name: 'Phone', type: 'string' },
                { name: 'Mobile', type: 'string' },
                { name: 'Detail', type: 'string' },
                { name: 'Bossid', type: 'number' },
                { name: 'BossName', type: 'string' }
            ]
        };

    var dataAdapter = new $.jqx.dataAdapter(source);

    $("#contactList").jqxGrid(
        {
            //width: 1000,
            width: '100%',
            source: dataAdapter,
            pageable: true,
            autoheight: true,
            autorowheight: true,
            sortable: true,
            altrows: true,
            enabletooltips: true,
            editable: false,
            selectionmode: 'singlerow',
            pagesizeoptions: [10, 15, 20, 25, 30, 35, 40, 45, 50],
            //showfilterrow: true,
            filterable: true,
            pagesize: 15,
            theme: "metroCustom",//테마설정
            //columnsheight: '40px',//헤더 높이 설정
            localization: GetLocalizationString(),
            //virtualmode: true,      // 목록이 매우 많을 경우 일부분만 렌더링해서 보여주고 다른 페이지를 불러올 떄마다 렌더링한다.
            //rendergridrows: rendergridrows,
            columns: [
                { text: "ContactID", datafield: "Contactid", hidden: true },
                { text: "이름", datafield: "Name", align: 'center', cellsalign: 'left', width: '20%', minwidth: '100px' },
                { text: '부서', datafield: 'Department', align: 'center', cellsalign: 'center', width: '10%', minwidth: '80px' },
                { text: "직급", datafield: "Title", align: 'center', cellsalign: 'center', width: '10%', minwidth: '80px' },
                { text: '회사명', datafield: 'Account', align: 'center', cellsalign: 'center', width: '15%', minwidth: '100px' },
                { text: 'E-Mail', datafield: 'Email', align: 'center', cellsalign: 'left', width: '15%', minwidth: '100px' },
                { text: '전화번호', datafield: 'Phone', align: 'center', cellsalign: 'center', width: '15%', minwidth: '100px' },
                { text: '휴대전화', datafield: 'Mobile', align: 'center', cellsalign: 'center', width: '15%', minwidth: '100px' }
            ]
        });
}