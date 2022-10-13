
$(document).ready(function () {
  //  dropzoneInit();
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
                $("#dialogContent").html(result.responseText);
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
                $("#ShowComponentsChangedialogContent").html(result.responseText);
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

//初始化dropzone
function dropzoneInit() {
    $.each($(".dropzone"), function () {
        var zone = $(this);
        $(this).dropzone({
            url: "/Ajax/AjaxFileUpload",
            method:'post',
            dictDefaultMessage: "把檔案拉進來或點我選檔案(可多檔案) <br/>※不支援.msg檔上傳, 請<a href='http://tsti-bpm01/files/msg_to_mht.png' target='_blank'> 轉換成MHT</a>或圖檔。",
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

                    if (!tIsExits) {
                        done("you don't.");
                        alert("檔名不合法！請依照此格式：\n ①請款通知: 請款單號(必要)+_+文件敘述，例如. 3000700991_客戶訂單 \n ②預開發票: 銷售訂單(必要)+_+文件敘述，例如. 1230015555_預開證明");
                    } else {
                        done();
                    }

                } else {
                    done();
                }
            },
            acceptedFiles: ".jpg,.png,.jpge,.gif,.pdf,.doc,.docx,.xls,.xlsx,.txt,.ppt,.pptx,.zip,.rar,.7z,.csv,.mht",
            createImageThumbnails: false,
            init: function () {
                this.on("success", function (result) {
                    var obj = $.parseJSON(result.xhr.responseText);

                    var doc = "<div class=\"file-box\" id='" + obj.cFile_ID + "'>";
                    doc += "<div class=\"file\">";
                    doc += "<a href='http://" + window.location.host + "/files/" + obj.cFile_NewName + "'" + (("jpg png bpmp".indexOf(obj.cFile_FileType) != -1) ? "" : "download='" + obj.cFile_OriginalName + "'") + " target='_blank'>";
                    doc += "<span class=\"corner\"></span>";
                    if ("jpg png bpmp".indexOf(obj.cFile_FileType) != -1) {
                        doc += " <div class=\"image\"><img alt=\"\" class=\"img-responsive\" src=\"http://" + window.location.host + "/files/" + obj.cFile_NewName + "\"></div>";
                    } else {
                        doc += "<div class=\"icon\"><i class=\"fa fa-file\"></i></div>";
                    }


                    if ($(zone).data("type") == null) {
                        doc += "<div class=\"file-name\">" + obj.cFile_OriginalName;
                        doc += "<a href=\"javascript:delFile('" + obj.cFile_ID + "', 'ctl00_MainContent_hid_file' + '" + zone.prop("id") + "')\" class='btn btn-xs btn-danger btn-del'>刪除</a></div></a>";
                        doc += " </div></div>";

                        var fileZoneObj = $("#ctl00_MainContent_file" + zone.prop("id"))
                        fileZoneObj.append(doc);

                        var hidFiles = $("#ctl00_MainContent_hid_file" + zone.prop("id"))
                        hidFiles.val(hidFiles.val() + obj.cFile_ID + ",");
                        console.log(hidFiles.val());

                    } else if ($(zone).data("type") == "type2") { //動態產生上傳區塊使用
                        doc += "<div class=\"file-name\">" + obj.cFile_OriginalName;
                        doc += "<a href=\"javascript:delFile('" + obj.cFile_ID + "', '" + zone.prop("id").substr(0, zone.prop("id").lastIndexOf('zone')) + 'hid_file' + zone.prop("id").substr(zone.prop("id").lastIndexOf('zone')) + "')\" class='btn btn-xs btn-danger btn-del'>刪除</a></div></a>";
                        doc += " </div></div>";

                        var fileZoneObj = $("#" + zone.prop("id").substr(0, zone.prop("id").lastIndexOf('zone')) + 'file' + zone.prop("id").substr(zone.prop("id").lastIndexOf('zone')))
                        fileZoneObj.append(doc);

                        var hidFiles = $("#" + zone.prop("id").substr(0, zone.prop("id").lastIndexOf('zone')) + 'hid_file' + zone.prop("id").substr(zone.prop("id").lastIndexOf('zone')))
                        hidFiles.val(hidFiles.val() + obj.cFile_ID + ",");
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
function generatorFileDiv() {
    $.each($(".fileids"), function (i, hidFiles) {
        if ($(hidFiles).val().length > 1) {
            var fileZoneObj = $("#" + $(hidFiles).prop("id").replace("hid_", ""));
            var fileids = $(hidFiles).val().split(",");
            for (var i in fileids) {
                $.ajax({
                    url: "../AjaxHandler.ashx",
                    type: 'post',
                    dataType: 'json',
                    data: { functionName: "findFileById", fileId: fileids[i] },
                    success: function (result) {
                        var doc = "<div class=\"file-box\" id='" + result.cFile_ID + "'>";
                        doc += "<div class=\"file\">";
                        doc += "<a href='http://" + window.location.host + "/files/" + result.cFile_NewName + "' " + (("jpg png bpmp".indexOf(result.cFile_FileType) != -1) ? "" : "download='" + result.cFile_OriginalName + "'") + " target='_blank'>";
                        doc += "<span class=\"corner\"></span>";
                        if ("jpg png bpmp".indexOf(result.cFile_FileType) != -1) {
                            doc += " <div class=\"image\"><img alt=\"\" class=\"img-responsive\" src=\"http://" + window.location.host + "/files/" + result.cFile_NewName + "\"></div>";
                        } else {
                            doc += "<div class=\"icon\"><i class=\"fa fa-file\"></i></div>";
                        }

                        doc += "<div class=\"file-name\">" + result.cFile_OriginalName;
                        doc += "<a href=\"javascript:delFile('" + result.cFile_ID + "', '" + $(hidFiles).prop("id") + "')\" class='btn btn-xs btn-danger btn-del'>刪除</a></div></a>";
                        doc += " </div></div>";
                        fileZoneObj.append(doc);
                        console.log(result.cFile_OriginalName);
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