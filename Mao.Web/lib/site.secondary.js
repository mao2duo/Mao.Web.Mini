var UI = (function (window) {
    var blockCount = 0;
    var blockShowing = false;
    /** 存放元件相關設定 */
    var component = {
        datepicker: {
            defaultOptions: {
                format: "yyyy-mm-dd",
                autoclose: true,
                clearBtn: true,
                language: APP.settings.language,
            }
        },
        bootstrapTable: {
            defaultOptions: {
                search: false,
                pagination: true,
                sidePagination: "client",
                pageList: [10, 25, 100, "All"],
                theadClasses: "table-primary",
                classes: "table table-bordered table-striped table-hover"
            },
            /** 顯示行數 */
            rowNumberFormatter: function (value, row, index, field) {
                return (index + 1);
            },
            /** 根據布林值顯示打勾圖示 */
            booleanFormatter: function (value, row, index, field) {
                if ((typeof value == "boolean" && value == true)
                    || (typeof value == "number" && value == 1)
                    || (typeof value == "string" && (value == "1" || value.toLowerCase() == "t" || value.toLowerCase() == "true"))) {
                    return '<div class="text-center"><i class="fas fa-check"></i></div>';
                }
                return "";
            },
            /** 顯示 HTML 編碼後的文字 */
            htmlEncodeFormatter: function (value, row, index, field) {
                return htmlEncode(value);
            },
            /** 顯示具有斷行效果的文字 */
            prelineFormatter: function (value, row, index, field) {
                return '<div style="white-space: pre-line">' + htmlEncode(value) + '</div>';
            }
        }
    };
    var render = {
        /**
         * 以 Mustache 來渲染 HTML 樣板
         * @param {string} templateId
         * @param {object} data
         */
        mustache: function (templateId, data) {
            return new Promise(function (resolve, reject) {
                var template = document.getElementById(templateId).innerHTML;
                var rendered = Mustache.render(template, data);
                return resolve(rendered);
            });
        },
        /**
         * 以 Razor 的 helper 來渲染 HTML 樣板
         * @param {string} viewPathCiphertext
         * @param {string} heplerName
         * @param {Array} heplerParameters
         */
        razorHelper: function (viewPathCiphertext, heplerName) {
            var heplerParameters = Array.prototype.slice.call(arguments, 2);
        //razorHelper: function (viewPathCiphertext, heplerName, ...heplerParameters) {
            return new Promise(function (resolve, reject) {
                $.ajax({
                    type: "POST",
                    async: true,
                    cache: false,
                    url: APP.settings.razorHelperUrl,
                    data: {
                        viewPathCiphertext: viewPathCiphertext || APP.settings.viewPathCiphertext,
                        helperName: heplerName,
                        helperParameters: JSON.stringify(heplerParameters)
                    },
                    dataType: 'json',
                }).done(function (response) {
                    return resolve(response);
                }).fail(function (xhr, textStatus, errorThrown) {
                    return reject({ xhr, textStatus, errorThrown });
                });
            });
        },
    };

    /** 畫面遮罩 (開啟) */
    function block() {
        if (blockCount === 0) {
            if (blockShowing == false) {
                blockShowing = true;
                $(".ui-block").show(600, function () {
                    blockShowing = false;
                });
            }
        }
        blockCount++;
    }
    /** 畫面遮罩 (關閉) */
    function unblock() {
        if (blockCount > 0) {
            blockCount--;
        }
        if (blockCount === 0) {
            $(".ui-block").hide();
        }
    }
    /**
     * 通知訊息
     * @param {string} message
     * @param {"primary" | "secondary" | "success" | "danger" | "warning" | "info" | "light" | "dark"} style
     * @param {Number} delay
     */
    function notice(message, style, delay) {
        UI.render.razorHelper(APP.settings.layoutPathCiphertext, "RenderNotice", message, style || "danger")
            .then(function (rendered) {
                var $notice = $(rendered);
                $(".ui-notices").append($notice);
                $notice.toast({
                    delay: delay || 3000
                });
                $notice.toast("show");
            });
    }
    /**
     * 通知訊息 (存放進 cookie，預設在導頁之後顯示)
     * @param {string} message
     * @param {"primary" | "secondary" | "success" | "danger" | "warning" | "info" | "light" | "dark"} style
     * @param {Number} delay
     */
    function noticeEnqueue(message, style, delay) {
        var notices = JSON.parse(getCookie("__notices") || "[]");
        notices.push({
            message: message,
            style: style,
            delay: delay
        });
        setCookie("__notices", JSON.stringify(notices), 0.0007);
    }
    /** 通知訊息 (顯示存放在 cookie 的項目，預設在頁面載入時執行) */
    function noticeDequeue() {
        JSON.parse(getCookie("__notices") || "[]").forEach(function (x) {
            notice(x.message, x.style, x.delay);
        });
        deleteCookie("__notices");
    }
    /**
     * 提示訊息對話框
     * @param {string} message
     * @param {string} title
     */
    function alert(message, title) {
        return new Promise(function (resolve, reject) {
            var $container = $(".ui-alert");
            var $buttonOk = $container.find(".ui-alert-ok");
            $buttonOk.off("click");
            $buttonOk.on("click", function () {
                $buttonOk.off("click");
                $container.hide();
                return resolve();
            });
            if (typeof title == "string" && title.length) {
                $container.find(".ui-alert-title-container").show();
                $container.find(".ui-alert-title").text(title);
            }
            else {
                $container.find(".ui-alert-title-container").hide();
                $container.find(".ui-alert-title").text("");
            }
            $container.find(".ui-alert-message").text(message);
            $container.show();
        });
    }
    /** 確認對話框 */
    function confirm(message, title) {
        return new Promise(function (resolve, reject) {
            var $container = $(".ui-confirm");
            var $buttonOk = $container.find(".ui-confirm-ok");
            var $buttonCancel = $container.find(".ui-confirm-cancel");
            $buttonOk.off("click");
            $buttonOk.on("click", function () {
                $buttonOk.off("click");
                $container.hide();
                return resolve(true);
            });
            $buttonCancel.off("click");
            $buttonCancel.on("click", function () {
                $buttonCancel.off("click");
                $container.hide();
                return resolve(false);
            });
            if (typeof title == "string" && title.length) {
                $container.find(".ui-confirm-title-container").show();
                $container.find(".ui-confirm-title").text(title);
            }
            else {
                $container.find(".ui-confirm-title-container").hide();
                $container.find(".ui-confirm-title").text("");
            }
            $container.find(".ui-confirm-message").text(message);
            $container.show();
        });
    }
    /** 針對 401 判斷為未登入時，導向登入頁 */
    function unauthorizedHandling(xhr, textStatus, errorThrown) {
        if (xhr.status == 401 && !APP.settings.userToken) {
            UI.confirm("需要登入才能繼續，要立即前往登入頁面嗎")
                .then(function (isConfirmed) {
                    if (isConfirmed) {
                        location.href = APP.settings.loginUrl;
                    }
                });
            return true;
        }
        return false;
    }
    /** 顯示 ModelState 的驗證訊息 */
    function modelStateErrorHandling(xhr, textStatus, errorThrown) {
        // 取得 ModelState 的 name 對應訊息區塊的方法
        function getModelStateAlerts(name) {
            return $("[data-model-state-alert]").filter(function () {
                return $(this).attr("data-model-state-alert").split(",").map(function (x) {
                    return x.trim();
                }).indexOf(name) >= 0;
            });
        }
        // 判斷回傳物件是否有 ModelState 需要處理
        if (xhr.responseJSON && xhr.responseJSON.ModelState) {
            var regex = new RegExp("(request\.)?(.+)");
            for (var modelStateName in xhr.responseJSON.ModelState) {
                var modelStateNameMatch = regex.exec(modelStateName);
                var name = modelStateNameMatch[2];
                // 取得 ModelState 對應的輸入框
                var $inputs = $("[data-model-state-input='" + name + "'], " + "[name='" + name + "'], #" + name);
                if ($inputs.length == 0) {
                    console.warn("ModelState unhandled: " + name);
                    continue;
                }
                // 為輸入框加上 data-model-state-input 屬性
                $inputs.not("[data-model-state-input]").attr("data-model-state-input", name);
                // 取得 ModelState 對應的訊息區塊
                var $alerts = getModelStateAlerts(name);
                // 建立預設的訊息區塊
                if ($alerts.length == 0) {
                    var alerts = [];
                    $inputs.each(function () {
                        var $alert = $('<div class="alert alert-danger mt-1 mb-0" role="alert"></div>');
                        $alert.attr("data-model-state-alert", name);
                        $(this).after($alert);
                        alerts.push($alert[0]);
                    });
                    $alerts = $(alerts);
                }
                // 在訊息區塊中放入驗證訊息
                $alerts.empty();
                xhr.responseJSON.ModelState[modelStateName].forEach(function (text) {
                    $alerts.append("<div>" + htmlEncode(text) + "</div>");
                });
                $alerts.show();
                // 當輸入框的值改變時，隱藏訊息區塊
                var eventHideAlerts = function () {
                    // 從 data-model-state-input 的值重新取得對應的訊息區塊
                    getModelStateAlerts($(this).attr("data-model-state-input")).hide();
                    $inputs.off("change keyup", eventHideAlerts);
                };
                $inputs.on("change keyup", eventHideAlerts);
            }
            return true;
        }
        return false;
    }
    /** 以通知顯示錯誤訊息 */
    function noticeErrorHandling(xhr, textStatus, errorThrown) {
        notice("請求發生異常：" + xhr.statusText, "danger");
        return true;
    }

    window.addEventListener('DOMContentLoaded', function (event) {
        noticeDequeue();
    });
    return {
        component: component,
        render: render,
        errorHandlings: {
            unauthorizedHandling: unauthorizedHandling,
            modelStateErrorHandling: modelStateErrorHandling,
            noticeErrorHandling: noticeErrorHandling
        },
        block: block,
        unblock: unblock,
        notice: notice,
        noticeEnqueue: noticeEnqueue,
        noticeDequeue: noticeDequeue,
        alert: alert,
        confirm: confirm
    };
})(typeof window !== "undefined" ? window : this);

var API = (function (window) {
    var uploadCount = 0;
    var isErrorHandled = false;

    /** 透過 ajax 呼叫 Server 端的 API */
    function http(httpMethod, url, data) {
        return new Promise(function (resolve, reject) {
            if (httpMethod.toUpperCase() != "GET") {
                if (uploadCount > 0) {
                    UI.notice("檔案上傳未完成，請稍後再進行操作。", "danger");
                    return;
                }
            }
            UI.block();
            $.ajax({
                type: httpMethod,
                async: true,
                cache: false,
                url: APP.settings.apiBaseUrl.substring(0, APP.settings.apiBaseUrl.length - 1) + url,
                headers: {
                    Authorization: "Bearer " + APP.settings.userToken
                },
                data: data,
                dataType: 'json',
            }).done(function (response) {
                return resolve(response);
            }).fail(function (xhr, textStatus, errorThrown) {
                isErrorHandled = false;
                setTimeout(function () {
                    if (isErrorHandled == false) {
                        isErrorHandled = UI.errorHandlings.unauthorizedHandling(xhr, textStatus, errorThrown);
                    }
                    if (isErrorHandled == false) {
                        isErrorHandled = UI.errorHandlings.modelStateErrorHandling(xhr, textStatus, errorThrown);
                    }
                    if (isErrorHandled == false) {
                        isErrorHandled = UI.errorHandlings.noticeErrorHandling(xhr, textStatus, errorThrown);
                    }
                }, 0);
                return reject({ xhr, textStatus, errorThrown });
            }).always(function () {
                UI.unblock();
            });
        });
    };
    function get(url, data) { return http("GET", url, data); };
    function post(url, data) { return http("POST", url, data); };
    function put(url, data) { return http("PUT", url, data); };
    function del(url, data) { return http("DELETE", url, data); };
    return {
        /** 是否已處理 API 的錯誤 */
        set isErrorHandled(value) {
            isErrorHandled = value;
        },
        http: http,
        get: get,
        post: post,
        put: put,
        delete: del,
        user: {
            register: function (data) { return post("/api/User/Register", data); },
        },
        menu: {
            list: function (data) { return get("/api/Menu/List", data); },
            get: function (data) { return get("/api/Menu", data); },
            create: function (data) { return post("/api/Menu", data); },
            update: function (data) { return put("/api/Menu", data); },
            delete: function (data) { return del("/api/Menu", data); }
        },
        database: {
            list: function (data) { return get("/api/Database/List", data); },
            get: function (data) { return get("/api/Database", data); },
            create: function (data) { return post("/api/Database", data); },
            update: function (data) { return put("/api/Database", data); },
            delete: function (data) { return del("/api/Database", data); }
        },
        databaseTable: {
            list: function (data) { return get("/api/DatabaseTable/List", data); },
            get: function (data) { return get("/api/DatabaseTable", data); },
            create: function (data) { return post("/api/DatabaseTable", data); },
            update: function (data) { return put("/api/DatabaseTable", data); },
            updateList: function (data) { return put("/api/DatabaseTable/List", data); },
            delete: function (data) { return del("/api/DatabaseTable", data); },
            convertToSqlTablesSerialized: function (data) { return post("/api/DatabaseTable/ConvertToSqlTablesSerialized", data); },
            convertFromSqlTablesSerialized: function (data) { return post("/api/DatabaseTable/ConvertFromSqlTablesSerialized", data); },
            getSerializeSqlTablesScript: function (data) { return get("/api/DatabaseTable/SerializeSqlTablesScript", data); },
            getUpdateTablesDescScript: function (data) { return get("/api/DatabaseTable/UpdateTablesDescScript", data); },
        },
        databaseTableColumn: {
            list: function (data) { return get("/api/DatabaseTableColumn/List", data); },
            get: function (data) { return get("/api/DatabaseTableColumn", data); },
            create: function (data) { return post("/api/DatabaseTableColumn", data); },
            update: function (data) { return put("/api/DatabaseTableColumn", data); },
            updateList: function (data) { return put("/api/DatabaseTableColumn/List", data); },
            delete: function (data) { return del("/api/DatabaseTableColumn", data); },
            convertToSqlColumnsSerialized: function (data) { return post("/api/DatabaseTableColumn/ConvertToSqlColumnsSerialized", data); },
            convertFromSqlColumnsSerialized: function (data) { return post("/api/DatabaseTableColumn/ConvertFromSqlColumnsSerialized", data); },
            getSerializeSqlColumnsScript: function (data) { return get("/api/DatabaseTableColumn/SerializeSqlColumnsScript", data); },
            getUpdateColumnsDescScript: function (data) { return get("/api/DatabaseTableColumn/UpdateColumnsDescScript", data); },
        },
        generate: {
            zip: function (data) { return post("/api/Generate/Zip", data); },
        },
        generateInput: {
            list: function (data) { return get("/api/GenerateInput/List", data); },
            get: function (data) { return get("/api/GenerateInput", data); },
            create: function (data) { return post("/api/GenerateInput", data); },
            update: function (data) { return put("/api/GenerateInput", data); },
            delete: function (data) { return del("/api/GenerateInput", data); }
        }
    };
})(typeof window !== "undefined" ? window : this);