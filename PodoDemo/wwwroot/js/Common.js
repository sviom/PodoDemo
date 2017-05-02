
//Trim
String.prototype.Trim = function () {
    return this.replace(/^\s\s*/, "").replace(/\s\s*$/, "")
}

//ReaplceAll
String.prototype.replaceAll = function (str1, str2) {
    var temp_str = this.trim();
    temp_str = temp_str.replace(eval("/" + str1 + "/gi"), str2);
    return temp_str;
}

//숫자여부 반환
function IsNumber(source) {
    return !isNaN(source);
}

//아이디 정규식
function IsIDType(source) {
    var reg_id = /^[a-zA-Z0-9_]{4,20}$/;
    if (!reg_id.test(source)) {
        return false;
    }
    return true;
}

//이메일 유효성 검사
function IsEmailType(source) {
    var reg_email = /^([0-9a-zA-Z_\.-]+)@([0-9a-zA-Z_-]+)(\.[0-9a-zA-Z_-]+){1,2}$/;
    if (!reg_email.test(source)) {
        return false;
    }
    return true;
}

//휴대전화/전화 유효성 검사
function IsPhoneType(source) {
    var reg_phone = /^[0-9]{2,3}-[0-9]{3,4}-[0-9]{4}/;
    if (!reg_phone.test(source)) {
        return false;
    }
    return true;
}

//사업자번호 유효성검사
function IsBizNumType(source) {
    var reg_biznum = /^[0-9]{3}-[0-9]{2}-[0-9]{5}/;
    if (!reg_biznum.test(source)) {
        return false;
    }
    return true;
}

//날짜 유효성 체크
function IsDateType(source) {
    var reg_date = /^[0-9]{4}-[0-9]{1,2}-[0-9]{1,2}/;
    if (!reg_date.test(source)) {
        return false;
    }
    return true;
}

//메뉴 이동하기(대메뉴 번호, URL, 전송하고자 하는 Form ID, 목록에서 오는 경우는 GRID ID를 넣어준다.)
function MoveMenu(menuNum, URL, formName, gridID) {
    _postbackURL = URL;
    _postbackMenuNum = menuNum;

    // 메뉴 관련
    var menus = document.getElementsByName("BigMenu");
    var menuCount;

    // 메뉴 초기화 [S]
    for (menuCount = 0; menuCount < menus.length; menuCount++) {
        menus[menuCount].style.fontWeight = "";
        menus[menuCount].style.backgroundColor = "";
        menus[menuCount].style.color = "white";
        menus[menuCount].style.backgroundRepeat = "no-repeat";
        menus[menuCount].style.backgroundPosition = "20px";
    }
    // 메뉴 초기화 [E]

    // 페이지 이동
    $("#" + formName).attr("method", "POST");
    $("#" + formName).attr("action", URL);
    $("#" + formName).submit();
}

//팝업 메뉴인 경우 팝업을 호출 한다.
function MovePopupMenu(url) {
    //var wX = screen.availWidth;
    //var wY = screen.availHeight;
    //wY = (wY-38);
    var wX = 1024;
    var wY = 768;

    var winIntro = window.open(url, "PodoCRM_MenuPopup", "width=" + wX + ", height=" + wY + ", scrollbars=yes, status=yes, resizable=yes, direction=yes, location=no, menubar=no, toolbar=no, titlebar=yes");

    winIntro.focus();
}

//옵션마스터를 불러와 배열로 반환합니다.
//serachKey : 옵션마스터 대분류 ID
//extraParam : callbackFnc에서 사용할 추가 파라미터
//callbackFnc : 결과를 전달할 함수명
//selval : 기본 선택값 필요시
function GetOptionDDL(searchKey, extraParam, callbackFnc, selval) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Service/CommonService.svc/GetOptionDDL",
        data: JSON.stringify({
            "searchKey": searchKey
        }),
        async: false,
        dataType: "json",
        success: function (result) {
            callbackFnc(extraParam, result, selval);
        },
        error: function (data) {
            alert("[!ERROR]\n" + data.responseText);
        }
    });
}

//메뉴목록을 불러와 배열로 반환합니다.
//extraParam : callbackFnc에서 사용할 추가 파라미터
//calbackFnc : 결과값을 전달할 콜백 함수
//selval : 기본 선택값 필요시
function GetMenuOptionDDL(extraParam, callbackFnc, selval) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Service/CommonService.svc/GetMenuOptionDDL",
        async: false,
        dataType: "json",
        success: function (result) {
            callbackFnc(extraParam, result, selval);
        },
        error: function (data) {
            alert("[!ERROR]\n" + data.responseText);
        }
    });
}

//사용자 리스트 바인딩 (2016/11/16 강한별)
//searchKey : 해당 사용자 부서 ID
//userID : 현재 로그인된 사용자 ID
//extraParam: callbackFnc에서 사용할 추가 파라미터(여러개일 경우 배열로)
//callbakFnc: 결과값을 전달할 함수
//selval: 기본 선택값 필요시 
function GetUserDDL(searchKey, userID, extraParam, callbackFnc, selval) {
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/Service/CommonService.svc/GetAllUserDDL",
        data: JSON.stringify({
            "searchKey": searchKey
            , "userid": userID
        }),
        async: false,
        dataType: "json",
        success: function (result) {
            callbackFnc(extraParam, result, selval);
        },
        error: function (data) {
            alert("[!ERROR]\n" + data.responseText);
        }
    });
}

//DropDownList 관련 배열을 option 태그로 변환합니다.
//result: 드롭다운리스트 관련 배열
//isAll : 전체 항목 추가시 true, 아닐 경우 false
//isEmpty: 맨 위의 항목이 빈 항목이어야 할 경우 true, 아닐 경우 false
//isAuth: 시스템 사용자 등 사용자의 권한이 필요한 항목이 있을 경우
//selval: 기본 선택 값이 필요한 경우
function ConvertToOptionTag(result, isAll, isEmpty, isAuth, selval) {
    var optionHTML = '';

    if (isAll) {
        optionHTML += '<option value="">전체</option>';
    }

    if (isEmpty) {
        optionHTML += '<option value=""></option>';
    }

    for (var i = 0; i < result.length; i++) {

        if (isAuth == "True") {
            optionHTML += '<option value="' + result[i].Value + '">' + result[i].Text + '</option>';
        } else {
            if (result[i].Value === "13-2") {
                continue;
            } else if (selval == result[i].Text || selval == result[i].Value) {
                optionHTML += '<option value="' + result[i].Value + '\" selected==\"selected\"">' + result[i].Text + '</option>';
            }
            else {
                optionHTML += '<option value="' + result[i].Value + '">' + result[i].Text + '</option>';
            }
        }

        //if (selval == result[i].Text || selval == result[i].Value) {
        //    optionHTML += '<option value="' + result[i].Value + '\" selected==\"selected\"">' + result[i].Text + '</option>';
        //} else {
        //    optionHTML += '<option value="' + result[i].Value + '">' + result[i].Text + '</option>';
        //}
    }
    return optionHTML;
}

//DropDownList 관련 배열을 radio 태그로 변환합니다.
//name에는 radio 그룹 명칭
//selval에 기본값 설정, 없으면 옵션마스터의 isDefault로 기본 값 체크
function ConvertToRadioTag(result, name, selval) {
    var optionHTML = '';

    if (selval === "" || selval === null) {
        for (var i = 0; i < result.length; i++) {
            //alert("Value: " + result[i].Value + " / isDefault: " + result[i].isDefault);
            optionHTML += '<input type="radio" runat="server" name="' + name + '" value="' + result[i].Value + '"' + (result[i].isDefault ? ' checked="checked"' : '') + ' />&nbsp;' + result[i].Text + '&nbsp;&nbsp;&nbsp;';
        }
    }
    else {
        for (var i = 0; i < result.length; i++) {
            //alert("selval: " + selval + " / Value: " + result[i].Value);
            optionHTML += '<input type="radio" runat="server" name="' + name + '" value="' + result[i].Value + '"' + (selval === result[i].Value ? ' checked="checked"' : '') + ' />&nbsp;' + result[i].Text + '&nbsp;&nbsp;&nbsp;';
        }
    }

    return optionHTML;
}

//파일업로드 호출
function UploadAttachFile(formName, savePath, successFNC, errorFNC) {
    var _data = new FormData($('#' + formName)[0]);
    $.ajax({
        type: "POST",
        contentType: false,
        url: "/Pages/Common/FileUpload.aspx?savePath=" + savePath,
        data: _data,
        async: false,
        cache: false,
        dataType: "text",
        processData: false,
        success: function (result) {
            console.log(result);
            if (result.indexOf("[ERROR]") > -1) {
                errorFNC(result);
            }
            else {
                var jsonOBJ = eval("(" + result + ")");
                successFNC(jsonOBJ);
            }
        },
        error: function (data) {
            alert("[!ERROR]\n" + data.responseText);
        }
    });
}

//파일 다운로드창 호출
function AttachFileDownload(url, fileName) {
    var _downloadPopTitle = "podoCRM_FileDownloader";

    var form1 = document.createElement("form");
    form1.setAttribute("method", "POST");
    form1.setAttribute("action", "/Pages/Common/FileDownload.aspx");
    form1.setAttribute("target", _downloadPopTitle);
    document.body.appendChild(form1);

    //파일다운로드 URL Hidden 생성
    var formInput1 = document.createElement("input");
    formInput1.setAttribute("type", "hidden");
    formInput1.setAttribute("name", "downloadURL");
    formInput1.setAttribute("id", "downloadURL");
    formInput1.setAttribute("value", url);
    form1.appendChild(formInput1);

    if (typeof fileName == "undefined") {
        fileName = "";
    }

    //원본 파일명 Hidden 생성
    var formInput2 = document.createElement("input");
    formInput2.setAttribute("type", "hidden");
    formInput2.setAttribute("name", "orgFileName");
    formInput2.setAttribute("id", "orgFileName");
    formInput2.setAttribute("value", fileName);
    form1.appendChild(formInput2);

    form1.target = "workIFrame";
    form1.submit();
}

//숫자 천단위 콤마 찍기
function SetComma(source) {
    source = String(source);
    return source.replace(/(\d)(?=(?:\d{3})+(?!\d))/g, '$1,');
}

//숫자 천단위 콤마 풀기
function SetUnComma(source) {
    source = String(source);
    return source.replace(/[^\d]+/g, '');
}

/************************************************************************
함수명		: fn_WindowOpen
작성목적	: 일반 팝업창을 실행한다.
Return		: 일반창 오픈 후 결과값 반환
*************************************************************************/
this.WindowOpen = function fn_WindowOpen(url, formNm, width, height, x, y) {

    var wHeight = window.screen.availHeight;
    var wWidth = window.screen.availWidth;

    var strWidth = width;
    var strHeight = height;

    var ileft = 0;
    var itop = 0;

    if (x == undefined || y == undefined) {
        ileft = parseInt((parseInt(wWidth) - parseInt(strWidth)) / 2);
        itop = parseInt((parseInt(wHeight) - parseInt(strHeight)) / 2);
    } else {
        ileft = x;
        itop = y;
    }
    if (ileft < 0) ileft = 0;
    if (itop < 0) itop = 0;

    var strFeature = "toolbar=no,menubar=no,statusbar=no,scrollbars=no,resizable=no,location=no,";

    var form = window.open(url, formNm, strFeature + "height=" + strHeight + ",width=" + strWidth + ",top=" + itop + ",left = " + ileft);

    try {
        form.focus();
    } catch (e) {
    }

};


/************************************************************************
함수명		: fn_WindowOpenScroll
작성목적	: 스크롤이 가능한 일반 팝업창을 실행한다. 
Return		: 스크롤이 가능한 일반창 오픈 후 결과값 반환
수정내역	:
*************************************************************************/
this.WindowOpenScroll = function fn_WindowOpenScroll(url, formNm, width, height, x, y) {

    var wHeight = window.screen.availHeight;
    var wWidth = window.screen.availWidth;

    var strWidth = width;
    var strHeight = height;

    var ileft = 0;
    var itop = 0;

    if (x == undefined || y == undefined) {
        ileft = parseInt((parseInt(wWidth) - parseInt(strWidth)) / 2);
        itop = parseInt((parseInt(wHeight) - parseInt(strHeight)) / 2);
    } else {
        ileft = x;
        itop = y;
    }
    if (ileft < 0) ileft = 0;
    if (itop < 0) itop = 0;

    var strFeature = "toolbar=no,menubar=no,statusbar=no,scrollbars=yes,resizable=no,";

    var form = window.open(url, formNm, strFeature + "height=" + strHeight + ",width=" + strWidth + ",top=" + itop + ",left = " + ileft);
    try {
        form.focus();
    } catch (e) {
    }

};

//jqxwindow를 닫을때 모달 백그라운드를 닫는다.
function ModalBackgroundHide() {
    var obj = document.getElementsByClassName("jqx-window-modal");
    if (obj.length > 0) {
        for (var i = 0; i < obj.length; i++) {
            $(obj[i]).hide();
        }
    }
}

// 2016/11/15 강한별
// 알람페이지를 팝업으로 띄운다. (타입/해당 영역ID)
function CreateAlarm(RTYPE, RID) {
    WindowOpen('/Pages/MessageAlarm/AlarmView.aspx?RTYPE=' + RTYPE + "&RID=" + RID, 'message', 595, 595);
}

// 2016/11/15 강한별
// 메시지 페이지를 팝업으로 띄운다. (타입/해당 영역ID)
function CreateMessage(RTYPE, RID) {
    WindowOpen('/Pages/MessageAlarm/MessageView.aspx?RTYPE=' + RTYPE + "&RID=" + RID, 'message', 595, 595);
}

// 2016/11/15 강한별
// 영업기회에서 메모를 생성 시 호출
// 메모 생성 팝업창 호출한다.(메모를 적을 관계종류/관계 고유ID)
function Open_MemoCreatePop(relationObject, relationObjectID) {
    var _downloadPopTitle = "podoCRM_CreateMemoPop";

    var form1 = document.createElement("form");
    form1.setAttribute("method", "POST");
    form1.setAttribute("action", "/Pages/Memo/CreateMemo_Pop.aspx");
    form1.setAttribute("target", _downloadPopTitle);
    document.body.appendChild(form1);

    //관련 항목 설정
    var formInput1 = document.createElement("input");
    formInput1.setAttribute("type", "hidden");
    formInput1.setAttribute("name", "relationObject");
    formInput1.setAttribute("id", "relationObject");
    formInput1.setAttribute("value", relationObject);
    form1.appendChild(formInput1);

    //관련 항목 ID 설정
    var formInput2 = document.createElement("input");
    formInput2.setAttribute("type", "hidden");
    formInput2.setAttribute("name", "relationObjectID");
    formInput2.setAttribute("id", "relationObjectID");
    formInput2.setAttribute("value", relationObjectID);
    form1.appendChild(formInput2);

    var ileft = 0;
    var itop = 0;
    var wHeight = window.screen.availHeight;
    var wWidth = window.screen.availWidth;

    ileft = parseInt((parseInt(wWidth) - parseInt(615)) / 2);
    itop = parseInt((parseInt(wHeight) - parseInt(500)) / 2);

    if (ileft < 0) ileft = 0;
    if (itop < 0) itop = 0;

    window.open('', _downloadPopTitle, "width=645px, height=400px,top=" + itop + ",left = " + ileft); //url없이 타겟만 지정한 후 폼 전송

    form1.target = _downloadPopTitle;
    form1.submit();
}

//메모보기 팝업 호출
function Open_MemoViewPop(memoID) {
    var _viewPopTitle = "podoCRM_VieweMemoPop";

    var form1 = document.createElement("form");
    form1.setAttribute("method", "POST");
    form1.setAttribute("action", "/Pages/Memo/ViewMemo_Pop.aspx");
    form1.setAttribute("target", _viewPopTitle);
    document.body.appendChild(form1);

    //Memo ID설정
    var formInput1 = document.createElement("input");
    formInput1.setAttribute("type", "hidden");
    formInput1.setAttribute("name", "viewMemoID");
    formInput1.setAttribute("id", "viewMemoID");
    formInput1.setAttribute("value", memoID);
    form1.appendChild(formInput1);

    var ileft = 0;
    var itop = 0;
    var wHeight = window.screen.availHeight;
    var wWidth = window.screen.availWidth;

    ileft = parseInt((parseInt(wWidth) - parseInt(615)) / 2);
    itop = parseInt((parseInt(wHeight) - parseInt(500)) / 2);

    if (ileft < 0) ileft = 0;
    if (itop < 0) itop = 0;

    window.open('', _viewPopTitle, "width=645px, height=480px,top=" + itop + ",left = " + ileft); //url없이 타겟만 지정한 후 폼 전송

    form1.target = _viewPopTitle;
    form1.submit();
}
