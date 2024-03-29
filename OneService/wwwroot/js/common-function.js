﻿
$(document).ready(function () {
    //dropzoneInit();
    //generatorFileDiv();
});

//用借用單號查詢審核記錄
function showApproveHistory(cFormNo) {
    if (cFormNo != "") {
        $("#bulletinDialog").modal("show");
        $("#dialogTitle").html(cFormNo + " 借用記錄歷程");
        var url = "../ApproveHistory/Index";
        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            data: {
                cFormNo: cFormNo     //表單編號formNo                
            },
            success: function (result) {
            },
            complete: function (result) {
            }
        });
    } else {
        alert("借用單號不得為空！");
    }
}

//用借用單號查詢審核記錄
function showComponentsChangeDialog(cFormNo, cID, cItem, cSRID, cWERKS) {
    if (cSRID != "") {
        $("#ShowComponentsChangeDialog").modal("show");
        $("#ShowComponentsChangedialogTitle").html("項次：【" + cItem + "】零件更換");
        var url = "../Spare/ComponentsChange";
        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            data: {
                cFormNo: cFormNo,    //表單編號formNo                
                cID: cID,           //GUID(同備品明細檔的cID)
                cItem: cItem,       //項次(同備品明細檔的cItem)
                cSRID: cSRID,       //SRID
                cWERKS: cWERKS      //工廠別
            },
            success: function (result) {
            },
            complete: function (result) {
               
            }
        });
    } else {
        alert("服務請求ID不得為空！");
    }
}

//顯示確認提交的提示訊息
function showConfirmDialogMsg(title, content) {
    $("#ShowConfirmDialog").modal("show");
    $("#ShowConfirmdialogTitle").html(title);
    $("#ShowConfirmdialogContent").html(content);
}

//顯示倒數的提示訊息
function showCloseDialogMsg(title, content) {
    $("#ShowCloseDialog").modal("show");
    $("#ShowClosedialogTitle").html(title);
    $("#ShowClosedialogContent").html(content);
}

var timer; 

//顯示倒數秒數
function CountDown() {
    if (maxtime >= 0) {
        time = formatMinutes(maxtime);
        document.getElementById("div1").innerHTML = time;
        if (maxtime != 0) {
            --maxtime;
        } else {            
            clearInterval(timer);
            deleteLoginRecord(); //清除該表單Login記錄            
            showCloseDialogMsg("提醒", "您閒置的時間已超過10分鐘，請重新登入！")
        }
    }
}

function formatMinutes(minutes) {
    var day = parseInt(Math.floor(minutes / 86400));
    var hour = day > 0 ? Math.floor((minutes - day * 86400) / 3600) : Math.floor(minutes / 3600);
    var minute = hour > 0 ? Math.floor((minutes - day * 86400 - hour * 3600) / 60) : Math.floor(minutes / 60);
    var second = minute > 0 ? Math.floor(minutes - day * 86400 - hour * 3600 - minute * 60) : minutes;
    var time = "";
    //if (day > 0) time += day + "天";
    //if (hour > 0) time += hour + "小時";
    if (minute > 0) time += minute + "分 ";
    time += second + "秒";
    time = "距離登出還有: " + time;
    return time;
}

//移除上傳檔案
function delFile(fileId, fileZoneObjId) {
    if (confirm("確認刪除？")) {
        $("#" + fileId).hide("fade");
        $("#" + fileZoneObjId).val($("#" + fileZoneObjId).val().replace(fileId + ",", ""));
    }
}

//jquery3要用dropzone加這一行才行
Dropzone.autoDiscover = false;

//正式區
var prdReportUrl = "https://oneservice.etatung.com/files/";
var fileUrl = prdReportUrl;

//測試區
//var qasReportUrl = "http://tsticrmmbgw.etatung.com:8082/CSreport/";
//var fileUrl = qasReportUrl;

//初始化dropzone
function dropzoneInit(uploadUrl) {

    $.each($(".dropzone"), function () {
        var zone = $(this);
        $(this).dropzone({
            url: uploadUrl,
            method: 'post',
            dictDefaultMessage: "把檔案拉進來或點我選檔案(可選多檔) <br/>※不支援.msg檔上傳, 請<a href='http://tsti-bpm01/files/msg_to_mht.png' target='_blank'> 轉換成MHT</a>或圖檔。",
            accept: function (file, done) {
                console.log($(zone).data("namerule"));
                if ($(zone).data("namerule") != null) {
                    //檔名規則檢核
                    //file.name  上傳檔案的檔名（含副檔名）
                    var namerule = $(zone).data("namerule");
                    var pArray = namerule.split(",");
                    var tIsExits = false; //檔案是否合法

                    pArray.forEach(function (element) {
                        if (file.name.indexOf(element) != -1) {
                            tIsExits = true;
                        }
                    });

                } else {
                    done();
                }
            },
            acceptedFiles: ".jpg,.png,.jpeg,.gif,.pdf,.doc,.docx,.xls,.xlsx,.txt,.ppt,.pptx,.zip,.rar,.7z,.csv,.mht,.mp4,.mov",
            createImageThumbnails: false,
            init: function () {
                this.on("success", function (result) {
                    var obj = $.parseJSON(result.xhr.responseText);

                    var doc = "<div class=\"file-box\" id='" + obj.id + "'>";
                    doc += "<div class=\"file\">";
                    doc += "<a href='" + fileUrl + obj.fileName + "'" + ((".jpg .jpeg .png .bmp .gif".indexOf(obj.fileExt) != -1) ? "" : "download='" + obj.fileOrgName + "'") + " target='_blank'>";
                    doc += "<span class=\"corner\"></span>";
                    if (".jpg .jpeg .png .bmp .gif".indexOf(obj.fileExt) != -1) {
                        doc += " <div class=\"image\"><img alt=\"\" class=\"img-responsive\" src=\"http://" + window.location.host + "/files/" + obj.fileName + "\"></div>";
                    } else {
                        doc += "<div class=\"icon\"><i class=\"fa fa-file\"></i></div>";
                    }


                    if ($(zone).data("type") == null) {
                        doc += "<div class=\"file-name\">" + obj.fileOrgName;
                        doc += "<a href=\"javascript:delFile('" + obj.id + "', 'hid_file' + '" + zone.prop("id") + "')\" class='btn btn-xs btn-danger btn-del'>刪除</a></div></a>";
                        doc += " </div></div>";

                        var fileZoneObj = $("#file" + zone.prop("id"))
                        fileZoneObj.append(doc);

                        var hidFiles = $("#hid_file" + zone.prop("id"))
                        hidFiles.val(hidFiles.val() + obj.id + ",");
                        console.log(hidFiles.val());

                    } else if ($(zone).data("type") == "type2") { //動態產生上傳區塊使用
                        doc += "<div class=\"file-name\">" + obj.fileOrgName;
                        doc += "<a href=\"javascript:delFile('" + obj.id + "', '" + zone.prop("id").substr(0, zone.prop("id").lastIndexOf('zone')) + 'hid_file' + zone.prop("id").substr(zone.prop("id").lastIndexOf('zone')) + "')\" class='btn btn-xs btn-danger btn-del'>刪除</a></div></a>";
                        doc += " </div></div>";

                        var fileZoneObj = $("#" + zone.prop("id").substr(0, zone.prop("id").lastIndexOf('zone')) + 'file' + zone.prop("id").substr(zone.prop("id").lastIndexOf('zone')))
                        fileZoneObj.append(doc);

                        var hidFiles = $("#" + zone.prop("id").substr(0, zone.prop("id").lastIndexOf('zone')) + 'hid_file' + zone.prop("id").substr(zone.prop("id").lastIndexOf('zone')))
                        hidFiles.val(hidFiles.val() + obj.Id + ",");
                        console.log(hidFiles.val());
                    }

                });

                this.on("complete", function (file) {
                    this.removeFile(file);
                });
            }
        })
    })
}

//產生檔案區塊
function generatorFileDiv(functionUrl) {
    $.each($(".fileids"), function (i, hidFiles) {
        if ($(hidFiles).val().length > 1) {
            var fileZoneObj = $("#" + $(hidFiles).prop("id").replace("hid_", ""));
            var fileids = $(hidFiles).val().split(",");
            for (var i in fileids) {
                $.ajax({
                    url: functionUrl,
                    type: 'post',
                    dataType: 'json',
                    data: { fileId: fileids[i] },
                    success: function (result) {
                        var doc = "<div class=\"file-box\" id='" + result.id + "'>";
                        doc += "<div class=\"file\">";
                        doc += "<a href='" + fileUrl + result.fileName + "' " + ((".jpg .jpeg .png .bmp .gif".indexOf(result.fileExt) != -1) ? "" : "download='" + result.fileOrgName + "'") + " target='_blank'>";
                        doc += "<span class=\"corner\"></span>";
                        if (".jpg .jpeg .png .bmp .gif".indexOf(result.fileExt) != -1) {
                            doc += " <div class=\"image\"><img alt=\"\" class=\"img-responsive\" src=\"http://" + window.location.host + "/files/" + result.fileName + "\"></div>";
                        } else {
                            doc += "<div class=\"icon\"><i class=\"fa fa-file\"></i></div>";
                        }

                        doc += "<div class=\"file-name\">" + result.fileOrgName;
                        doc += "<a href=\"javascript:delFile('" + result.id + "', '" + $(hidFiles).prop("id") + "')\" class='btn btn-xs btn-danger btn-del'>刪除</a></div></a>";
                        doc += " </div></div>";
                        fileZoneObj.append(doc);
                        console.log(result.fileOrgName);
                    }
                })
                console.log(fileids[i]);
            }
        }


    })
}

function showDialogMsg(title, content) {
    $("#bulletinDialog").modal("show");
    $("#dialogTitle").html(title);
    $("#dialogContent").html(content);
}

//-----↓↓↓↓↓服務請求 ↓↓↓↓↓-----
//顯示確認提交的提示訊息
function ShowSRConfirmDialogMsg(title, content) {
    $("#ShowSRConfirmDialog").modal("show");
    $("#ShowSRConfirmdialogTitle").html(title);
    $("#ShowSRConfirmdialogContent").html(content);
}
//-----↑↑↑↑↑服務請求 ↑↑↑↑↑-----

//-----↓↓↓↓↓新增加共用可查詢郵遞區號和地址 ↓↓↓↓↓-----
//change縣市名稱
$('.changePostCity').change(function () {
    callPostAddress('City');
});

//change鄉鎮市區
$('.changePostTownship').change(function () {
    callPostAddress('Township');
});

////change路段(名)
//$('.changePostRoad').change(function () {
//    callPostAddress('Road');
//});

//開啟郵遞區號和地址查詢視窗
function openPostAddress() {
    callfindPostalaAddressInfo("", "", ""); //初始縣市名稱

    $("#div_PostAddressList").html("<br><br><br><br>");
    $("#div_PostAddress").modal('show');
}

//呼叫Ajax地址相關資訊
//tType：City.縣市名稱、Township.鄉鎮市區、Road.路段(名)
//keyword.  縣市名稱
//keyword2. 鄉鎮市區
//keyword3. 路段(名)
function callPostAddress(tType) {
    var ddl_City = $("#ddl_PostAddressCity");
    var ddl_Township = $("#ddl_PostAddressTownship");
    var ddl_Road = $("#ddl_PostAddressRoad");
    var ddl_No = $("#ddl_PostAddressNo");

    var keyword = ddl_City.val();

    var keyword2 = ddl_Township.val();
    if (keyword2 == undefined || keyword2 == null) {
        keyword2 = "";
    }

    var keyword3 = ddl_Road.val();
    if (keyword3 == undefined || keyword3 == null) {
        keyword3 = "";
    }

    if (tType == "City") {
        keyword2 = "";
        keyword3 = "";

        ddl_Township.html('');
        ddl_Road.html('');
        ddl_No.html('');

        ddl_Township.append(new Option("請選擇", ""));
    }
    else if (tType == "Township") {
        keyword3 = "";

        ddl_Road.html('');
        ddl_No.html('');

        ddl_Road.append(new Option("請選擇", ""));
    }
    else if (tType == "Road") {
        ddl_No.html('');
        ddl_No.append(new Option("請選擇", ""));
    }

    callfindPostalaAddressInfo(keyword, keyword2, keyword3); //呼叫取得該對應的地址名稱
}

//呼叫取得該對應的地址名稱
function callfindPostalaAddressInfo(keyword, keyword2, keyword3) {
    var ddl_City = $("#ddl_PostAddressCity");
    var ddl_Township = $("#ddl_PostAddressTownship");
    var ddl_Road = $("#ddl_PostAddressRoad");
    var ddl_No = $("#ddl_PostAddressNo");
    var url = "../ServiceRequest/findPostalaAddressInfo";

    $.ajax({
        url: url,
        type: 'post',
        dataType: 'json',
        data: { functionName: 'findPostalaAddressInfo', keyword: keyword, keyword2: keyword2, keyword3: keyword3 },
        success: function (result) {
            if (keyword == "") {
                ddl_City.html('');
                ddl_City.append(new Option("請選擇", ""));
            }

            $.each(result, function (i, idata) {
                if (keyword == "") {
                    ddl_City.append(new Option(idata, idata));
                }
                else {
                    if (keyword3 != "") {
                        ddl_No.append(new Option(idata, idata));
                    }
                    else if (keyword2 != "") {
                        ddl_Road.append(new Option(idata, idata));
                    }
                    else {
                        ddl_Township.append(new Option(idata, idata));
                    }
                }
            });
        }
    })
}

//查詢郵遞區號和地址
function findPostAddress() {
    var strMsg = "";
    var ddl_City = $("#ddl_PostAddressCity");         //縣市名稱
    var ddl_Township = $("#ddl_PostAddressTownship"); //鄉鎮市區
    var ddl_Road = $("#ddl_PostAddressRoad");         //路段(名)
    var ddl_No = $("#ddl_PostAddressNo");             //門牌號碼
    var url = "../ServiceRequest/findPostalaAddressAndCode";

    var keyword = ddl_City.val();
    if (keyword == undefined || keyword == null) {
        keyword = "";
    }

    var keyword2 = ddl_Township.val();
    if (keyword2 == undefined || keyword2 == null) {
        keyword2 = "";
    }

    var keyword3 = ddl_Road.val();
    if (keyword3 == undefined || keyword3 == null) {
        keyword3 = "";
    }

    var keyword4 = ddl_No.val();
    if (keyword4 == undefined || keyword4 == null) {
        keyword4 = "";
    }

    if (keyword == "" && keyword2 == "" && keyword3 == "" && keyword4 == "") strMsg += "請至少選擇一種查詢方式！\n";

    if (strMsg != "") {
        alert(strMsg);
    }
    else {
        $("#waitingImg1").show();
        $.ajax({
            url: url,
            type: 'post',
            dataType: 'json',
            data: { functionName: 'findPostalaAddressAndCode', keyword: keyword, keyword2: keyword2, keyword3: keyword3, keyword4: keyword4 },
            complete: function () {
                $("#waitingImg1").hide();
            },
            success: function (result) {
                var list = "";
                $.each(result, function (i, bean) {
                    var strCode = bean.code;
                    var strCity = bean.city;
                    var strTownship = bean.township;
                    var strRoad = bean.road;

                    var strNo = bean.no;
                    strNo = strNo.replace(/\s+/g, "&nbsp;");
                    strNo = strNo.replace(/\s+/g, "&nbsp;");
                    strNo = strNo.replace(/\"/g, "&quot;");

                    list += "<tr><td data-th='郵遞區號'><input id='" + bean.code + i + "' name='rdCode' type='radio' value='" + bean.code + "' onclick=selectPostalaAddress('" + strCode + "','" + strCity + "','" + strTownship + "','" + strRoad + "','" + strNo + "') /><label for='" + bean.code + "'>" + bean.code + "</label></td>"
                        + "<td data-th='縣市名稱'>" + strCity + "</td><td data-th='鄉鎮市區'>" + strTownship + "</td><td data-th='路段(名)'>" + strRoad + "</td><td data-th='門牌號碼'>" + strNo + "</td></tr>";
                });

                if (list != "") {
                    list = "<table class='table table-striple table-bordered table-hover dataTables-example'><tr><th>郵遞區號</th><th>縣市名稱</th><th>鄉鎮市區</th><th>路段(名)</th><th>門牌號碼</th></tr>"
                        + list + "</table>";
                }
                else {
                    list = "查無資料！";
                }

                $("#div_PostAddressList").html(list);
            }
        });
    }
}

//將選擇的郵遞區號和地址帶回原欄位中
function selectPostalaAddress(Code, City, Township, Road, No) {    
    var tbx_cAddContactCity = $("#tbx_cAddContactCity");         //城市
    var tbx_cAddContactAddress = $("#tbx_cAddContactAddress");    //地址   
    var tAddress = Township + Road; 

    tbx_cAddContactCity.val(City);
    tbx_cAddContactAddress.val(tAddress);

    $("#div_PostAddress").modal('hide');
}

//Ajax路段(名)關鍵字查詢
function RoadSearch() {   
    $(".changePostRoad").unbind();

    $(".changePostRoad").koala({
        delay: 300,
        keyup: function (event) {
            if (event.keyCode != 13 && event.keyCode != 37 && event.keyCode != 38 && event.keyCode != 39 && event.keyCode != 40) {
                var keyword = $(this).val();
                if (keyword.length > 1) {                   
                    var obj = $(this);
                    var Msg = "";
                    var ddl_PostAddressCity = $("#ddl_PostAddressCity");        //縣市名稱
                    var ddl_PostAddressTownship = $("#ddl_PostAddressTownship"); //鄉鎮市區
                    var tbx_cAddContactCity = $("#tbx_cAddContactCity");        //客戶聯絡人城市
                    var tbx_cAddContactAddress = $("#tbx_cAddContactAddress");   //客戶聯絡人詳細地址

                    if (ddl_PostAddressCity.val() == null || ddl_PostAddressCity.val() == "") {
                        Msg += "請選擇【縣市名稱】！\n";
                    }

                    if (ddl_PostAddressTownship.val() == null || ddl_PostAddressTownship.val() == "") {
                        Msg += "請選擇【鄉鎮市區】！\n";
                    }
                    
                    if (Msg != "") {
                        alert(Msg);
                    }
                    else
                    {
                        $.ajax({
                            url: '../ServiceRequest/findPostalRoadsInfo',
                            type: 'post',
                            dataType: 'json',
                            data: {
                                keyword: ddl_PostAddressCity.val(),
                                keyword2: ddl_PostAddressTownship.val(),
                                keyword3: obj.val()
                            },
                            success: function (result) {
                                objects = [];
                                $.each(result, function (i, idata) {
                                    objects[i] = {
                                        label: idata,
                                        idx: i,
                                        value: idata, //value這個值，一定要在label裡才能顯示出下拉                                    
                                        Road: idata
                                    };
                                });

                                //綁定foucs事件
                                obj.autocomplete({
                                    source: objects,
                                    select: function (event, ui) {                                        
                                        var tAddress = ddl_PostAddressTownship.val() + ui.item.Road;
                                        tbx_cAddContactCity.val(ddl_PostAddressCity.val());
                                        tbx_cAddContactAddress.val(tAddress); 
                                    }
                                }).bind('focus', function () { obj.autocomplete("search"); });

                                //開啟autocomplete選單
                                obj.focus();
                                $(".ui-autocomplete").css({ 'z-index': '2051', 'font-size': '16px' }); //可以正常顯示關鍵字查詢
                            }
                        })
                    }
                }
            }
        }
    })
}
//-----↑↑↑↑↑客戶聯絡人 ↑↑↑↑↑-----

//日期格式檢查(yyyy-MM-dd)
function dateValidationCheck(str) {
    let re = new RegExp("^([0-9]{4})[.-]{1}([0-9]{1,2})[.-]{1}([0-9]{1,2})$");
    let strDataValue;
    let infoValidation = true;
    if ((strDataValue = re.exec(str)) != null) {
        let i;
        i = parseFloat(strDataValue[1]);
        if (i <= 0 || i > 9999) {
            infoValidation = false;
        }
        i = parseFloat(strDataValue[2]);
        if (i <= 0 || i > 12) {
            infoValidation = false;
        }
        i = parseFloat(strDataValue[3]);
        if (i <= 0 || i > 31) {
            infoValidation = false;
        }
    } else {
        infoValidation = false;
    }
    
    return infoValidation;
}

//記錄登入人員匯出Excel的時間點Log
function SaveQueryLog(EventName) {
    var url = "../ServiceRequest/SaveQueryLog";

    $.ajax({
        url: url,
        type: 'post',
        dataType: 'json',
        data: {
            EventName: EventName
        },
        success: function (result) {

        }
    });
}