﻿$(function () {
    $('#Name').focus();
    $("#topImage").himallUpload(
    {
        title: 'Head pictures',
        imageDescript: 'Please upload pictures of 1920 * 500',
        displayImgSrc: $('#topImageBox').val(),
        imgFieldName: "TopImage"
    });

    
    //新增专题模块
    $('#moduleContainer').on('click', 'a.choose-goods', function () {
        var moduleIndex = $(this).attr('index');
        !moduleIndex && (moduleIndex = 0);
        var ids = null;
        if (moduleProducts[moduleIndex]) {//当前模块已选择过商品，则获取所有本模块商品的编号
            ids = [];
            $.each(moduleProducts[moduleIndex], function (i, product) {
                ids.push(product.id);
            });
        }

        $.productSelector.show(ids, function (selectedProducts) {
            //记录当前选中的商品
            moduleProducts[moduleIndex] = selectedProducts;
            $('tr[index="' + moduleIndex + '"] td[type="selectedNumber"]').html(selectedProducts.length);
        }, 'selleradmin');
    });

    $('#moduleContainer').on('click', 'a.a-del', function () {
        var moduleIndex = $(this).attr('index');
        removeModule(moduleIndex);
    });

    var moduleProductsString = $('#selectedProductIds').val();
    if (moduleProductsString) {
        moduleProducts = JSON.parse(moduleProductsString);
    }

});

//模块商品,用于装载各模块已选择的商品
var moduleProducts = [];



//添加模块
function addModule() {
    var container = $('#moduleContainer');
    var moduleIndex = $('#moduleContainer tr').length;//模块序号，用于定位模块

    var html = ' <tr index="' + moduleIndex + '">\
                            <td><input class="text-module" type="text" value="默认模块' + (moduleIndex + 1) + '" placeholder="Default module' + (moduleIndex + 1) + '" /></td>\
                            <td type="selectedNumber">unselected</td>\
                            <td class="td-operate"><span class="btn-a"><a class="choose-goods" index="' + moduleIndex + '">Select products</a><a class="a-del" index="' + moduleIndex + '">delete</a></span></td>\
                 </tr>';
    container.append(html);
}

//移除模块
function removeModule(moduleIndex) {
    var moduleName = $('#moduleContainer tr[index="' + moduleIndex + '"] input[type="text"]').val();
    $.dialog.confirm('Are you sure you want to delete ' + moduleName + '?', function () {
        $('#moduleContainer tr[index="' + moduleIndex + '"]').remove();
        $.dialog.tips('Delete successfully');
    });
}


function generateTopicInfo() {
    //专题对象
    var topic = {
        id: $('#topicId').val(),
        name: null,
        topImage: null,
        backgroundImage: null,
        topicModuleInfo: []
    };

    !topic.id && (topic.id = null);
    topic.name = $('#Name').val();
    topic.frontCoverImage = $("#topImage").himallUpload('getImgSrc');
    topic.topImage = $("#topImage").himallUpload('getImgSrc');
    topic.backgroundImage = $("#backgroudImage").himallUpload('getImgSrc');
    topic.topicModuleInfo = [];
    topic.tags = $('#Tags').val();

    var modules = $('#moduleContainer tr');
    $.each(modules, function (i, moduleItem) {
        var moduleIndex = $(moduleItem).attr('index');
        !moduleIndex && (moduleIndex = 0);
        var moduleInfo = {
            name: $(moduleItem).find('input').val(),
            moduleProductInfo: []
        };

        if (!moduleProducts[moduleIndex] || moduleProducts[moduleIndex].length == 0)
            throw new Error('“' + moduleInfo.name + '”Please select at least one product!');
        $.each(moduleProducts[moduleIndex], function (i, moduleProduct) {
            moduleInfo.moduleProductInfo.push({
                productId: moduleProduct.id,
                displaySequence: i + 1
            });
        });

        topic.topicModuleInfo.push(moduleInfo);
    });
    return topic;
}



function submitTopic() {
    var object;
    if ($('form').valid()) {
        try {
            object = generateTopicInfo();
            if (!object.topImage)
                $.dialog.alert("Please upload a header image");
            else {
                var objectString = JSON.stringify(object);
                var loading = showLoading();
                $.post('/selleradmin/mobileTopic/add', { topicJson: objectString }, function (result) {
                    loading.close();
                    if (result.success) {
                        $.dialog.tips('Save successfully',function(){location.href = '/selleradmin/mobileTopic/management';});
                    }
                    else
                        $.dialog.tips('Save failed!' + result.msg);
                }, "json");
            }
        }
        catch (e) {
            $.dialog.alert(e.message);
        }
    }
}