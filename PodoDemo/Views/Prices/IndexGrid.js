// 가격표 Index 그리드용
function SetPricesIndex(priceListRaw) {
    var source =
        {
            localdata: priceListRaw,
            datatype: "json",
            datafields:
            [
                { name: 'Priceid', type: 'number' },
                { name: 'Product', type: 'string', map: 'Product>Name' },
                { name: 'Cost', type: 'string' },
                { name: 'Prices', type: 'string' },
                { name: 'Currency', type: 'string' },
                { name: 'Ownerid', type: 'string' }
            ]
        };

    var dataAdapter = new $.jqx.dataAdapter(source);

    $("#priceList").jqxGrid({
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
        localization: GetLocalizationString(),
        columns: [
            //문자로 나오는 항목
            { text: "Priceid", datafield: "Priceid", hidden: true },
            { text: "제품", datafield: "Product", align: 'center', cellsalign: 'left', width: '30%', minwidth: '100px' },
            { text: "원가", datafield: 'Cost', align: 'center', cellsalign: 'center', width: '20%', minwidth: '100px', cellsformat: 'D' },
            { text: "판매가", datafield: "Prices", align: 'center', cellsalign: 'center', width: '20%', minwidth: '100px', cellsformat: 'D' },
            { text: "통화", datafield: "Currency", align: 'center', cellsalign: 'center', width: '10%', minwidth: '100px' },
            { text: '담당자', datafield: 'Ownerid', align: 'center', cellsalign: 'center', width: '20%', minwidth: '100px' },
        ]
    });
}