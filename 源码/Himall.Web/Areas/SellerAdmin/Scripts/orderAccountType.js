﻿
$(function () {
    var status = GetQueryString('status');
    if (status == 1) {
        typeChoose('1');
    }
    else if (status == 2) {
        typeChoose('2');
    } else {
        typeChoose()
    }


    function typeChoose(val) {

        //订单表格
        $("#listAccountType").hiMallDatagrid({
            url: './listAccountType',
            nowrap: false,
            rownumbers: true,
            NoDataMsg: 'No matching data',
            border: false,
            fit: true,
            fitColumns: true,
            pagination: true,
            idField: "OrderId",
            pageSize: 14,
            pagePosition: 'bottom',
            pageNumber: 1,
            queryParams: { accountType: val },
            //operationButtons: "#orderOperate",
            //onLoadSuccess: CheckStatus,
            columns:
			[[
               
				{
				    field: "OrderId", title: 'Order No.', width: 200, align: "left",
				    formatter: function (value, row, index) {
				        return '<img src="' + row.IconSrc + '" title="' + row.PlatformText + '订单" width="16" />' + value;
				    }
				},
				{ field: "UserName", title: "Buyer", width: 120, align: "left" },
				{ field: "OrderDate", title: "Order time", width: 140, align: "left" },
				{
				    field: "TotalPrice", title: "Total order", width: 120, align: "right",
				    formatter: function (value, row, index) {
				        var html = "<span class='ftx-04'>" + '$' + value.toFixed(2) + "</span>";
				        return html;
				    }
				},
			//{ field: "PaymentTypeName", title: "支付方式", width: 120, align: "left" },
			{
			    field: "OrderStatus", title: "Order status", width: 100, align: "center",
			    formatter: function (value, row, index) {
			        var html = ["<span class='ordstbox'>"];
			        switch (row.RefundStats) {
			            case null:
			            case 0:
			                break;
			            case 4:
			                break;
			            default:
			                html.push("<i class='refundico' title='有待处理退款'>有退款</i>");
			                break;
			        }
			        html.push(row.OrderStatus);
			        html.push("</span>");
			        return html.join("");
			    }
			},
            { field: "AccountType", title: "Settlement status", width: 130, align: "center" },
			{
			    field: "operation", operation: true, title: "operation",
			    formatter: function (value, row, index) {
			        var id = row.OrderId.toString();
			        var html = ["<span class=\"btn-a\">"];
			        html.push("<a href='./Detail/" + id + "'>view</a>");
			        
			        html.push("</span>");
			        return html.join("");
			    }
			}
			]]
        });
    }



    $('#searchButton').click(function (e) {
        searchClose(e);
        var startDate = $("#inputStartDate").val();
        var endDate = $("#inputEndDate").val();
        var orderId = $.trim($('#txtOrderId').val());
        if (isNaN(orderId)) {
            $.dialog.errorTips("Please enter right order number."); return false;
        }
        var userName = $.trim($('#txtUserName').val());
        var orderType = $("#orderType").val();
        $(".tabel-operate").find("label").remove();
        $("#listAccountType").hiMallDatagrid('reload', { startDate: startDate, endDate: endDate, orderId: orderId, userName: userName, paymentTypeGateway: $("#selectPaymentTypeName").val(), orderType: orderType });
    });

    $('.nav li').click(function (e) {
        try {
            searchClose(e);
            $(this).addClass('active').siblings().removeClass('active');
            if ($(this).attr('type') == 'statusTab') {//状态分类
                $('#txtOrderId').val('');
                $('#txtuserName').val('');
                
                $("#listAccountType").hiMallDatagrid('reload', { accountType: $(this).attr('value') || null });
            }
        }
        catch (ex) {
            alert();
        }
    });

});




function loadIframeURL(url) {
    var iFrame;
    iFrame = document.createElement("iframe");
    iFrame.setAttribute("src", url);
    iFrame.setAttribute("style", "display:none;");
    iFrame.setAttribute("height", "0px");
    iFrame.setAttribute("width", "0px");
    iFrame.setAttribute("frameborder", "0");
    document.body.appendChild(iFrame);
    // 发起请求后这个iFrame就没用了，所以把它从dom上移除掉
    //iFrame.parentNode.removeChild(iFrame);
    iFrame = null;
}


