/*
    <summary>Função para exibição de arquivos do tipo imagem.</summary>
    <param name="input">Elemento que contém o arquivo.</param>
    <param name="elementId">Elemento onde a imagem será exibida.</param>
*/

function show(input, elementId) {
    if (input.files && input.files[0]) {
        var filerdr = new FileReader();
        filerdr.onload = function (e) {
            $("#" + elementId).attr("src", e.target.result);
        }
        filerdr.readAsDataURL(input.files[0]);
    }
};

/*
    <summary>Função para atualização de grid.</summary>
    <param name="element">Elemento que realizou a chamada.</param>
    <param name="formId">Form que será requisitado.</param>
    <param name="tableName">Nome da tabela a ser atualizada.</param>
    <param name="urlTableUpdate">Url da lista a ser pesquisada.</param>
*/
function UpdateGrid(element, formId, tableName, urlTableUpdate) {
    var form = $("#" + formId);

    $(form).validate();

    $.ajax({
        type: $(form).attr("method"),
        url: $(form).attr("action"),
        data: $(form).serialize(),
        success: function () {
            $("#" + tableName).load(urlTableUpdate);
        },
        error: function (data) {
            message(data);
        }
    });
};

/*
    <summary>função para executar um formulário via Ajax.</summary>
    <param name="data">Elemento que realizou a chamada.</param>
    <param name="element">Form que será requisitado.</param>
*/
function Submit(element, formId) {
    var form = $("#" + formId);
    var formData = new FormData($(form)[0]);
    if ($(form).valid()) {
        $.ajax({
            type: $(form).attr("method"),
            url: $(form).attr("action"),
            data: formData,
            cache: false,
            contentType: false,
            processData: false,
            success: function () {
                location.reload();
            }
        });
    };
};

/*
    <summary>Inseri um item na promoção</summary>
*/
function InsertItemPromotion(url) {

    var parametros =
    {
        ProdutoMontadoId: $("#ProdutoMontadoId").val()
        ,TabelaDePrecoId: $("#TabelaDePrecoId").val()
    }

    $.post(url, parametros, function (retorno) {
        $("#promotionItem").load("/Admin/Promotions/PromotionItems", function () {
            if (retorno.success === false) {
                $("#msg").removeClass("hide");
                $("#msg").html(retorno.message);
                $("#msg").delay(1000).fadeOut(1000, function () {
                    $(this).addClass("hide");
                });
            }
            reset();
        });
    });
}

/*
    <summary>Remove um item na promoção</summary>
    <param name="url">Informar a url que será realizado o post</param>
    <param name="id">Recebe o index do item</param>
*/
function RemoveItemPromotion(url, id) {
    var parametros =
    {
        id: id
    }

    $.post(url, parametros, function (retorno) {
        $("#promotionItem").load("/Admin/Promotions/PromotionItems", function () {
            reset();
        });
    });
}

// Reinicializa as funções dos objetos.
function reset() {

    try {
        $("select").chosen();
        $(".table")
            .dataTable({
                "language": {
                    "emptyTable": "Não há dados disponíveis na tabela",
                    "info": "Exibindo página _PAGE_ de _PAGES_",
                    "infoEmpty": "Não há registros disponíveis",
                    "infoFiltered": "(filtrado do total de registros _MAX_)",
                    "infoPostFix": "",
                    "thousands": ",",
                    "lengthMenu": "Selecionar _MENU_ registros por página",
                    "loadingRecords": "Carregando...",
                    "processing": "Processando...",
                    "search": "Pesquisar: ",
                    "zeroRecords": "Desculpe ! Nenhum registro encontrado",
                    "paginate": {
                        "first": "Primeiro",
                        "last": "Último",
                        "next": "Próximo",
                        "previous": "Anterior"
                    },
                    "aria": {
                        "sortAscending": ": ativar para classificar coluna ascendente",
                        "sortDescending": ": ativar para classificar coluna descendente"
                    }
                }
            });
    } catch (e) {

    }

    /*
    <summary>Pesquisa as tabelas de preço do item selecionado, função executa ao clicar sobre o dropdown da tabela de preço.</summary>
    */

    $("#ProdutoMontadoId")
        .change(function () {
            $.post("/Admin/Promotions/GetTablePrices", { productId: $("#ProdutoMontadoId").val() }, function (result) {
                $("#TabelaDePrecoId").find('option').remove().end();
                $(result)
                    .each(function (i) {
                        $("<option>").val(result[i].id).text(result[i].code).appendTo("#TabelaDePrecoId");
                    });
                $("#TabelaDePrecoId").trigger("chosen:updated");
            });
        });
}

reset();

/*
    Controle dos itens da vitrine.
*/
$('.tipo').on('click', function () {
    var id = $(this).attr("control-value");
    var options = {};
    options.url = "/ProdutoVitrine/ObterCores";
    options.type = "GET";
    options.data = { "tamanhoId": id };
    options.datatype = "json";
    $('#ProdutoTamanhoId').val(id);
    options.success = function (data) {
        if (data.length == 0) return;
        /*
            $('.numeracao' + data[0].ProdutoId).html(data[0].Referencia);
        */
        // verificando se é o mesmo clicado, assim não continua a operação.
        var idSelected = $('div#' + data[0].ProdutoId + ".tipos span.Selected").attr("id");
        if (idSelected == id) return;
        // Limpa todos os elementos que representam a cor do tamanho do produto
        $("div#" + data[0].ProdutoId + ".cores").empty();
        $("span#" + id + ".tipo").removeClass("btn-default");
        $("span#" + id + ".tipo").addClass("btn-primary");
        $('div#' + data[0].ProdutoId + ".tipos span.Selected").removeClass('btn-primary').addClass('btn-default btn-sm').removeClass('Selected');
        $("span#" + id + ".tipo").addClass("Selected");
        $('#div-form-produto.' + data[0].ProdutoId + ' form').find('input#ProdutoTamanhoId').val(id);

        for (var i = 0; i < data.length; i++) {
            if (i == 0) {
                // Monta os elementos de cor,  para tamanho selecionado.

                var backgroundCor = "";

                if (data[i].ImagemCor == null || data[i].ImagemCor == "") {
                    backgroundCor = "linear-gradient(to left," + data[i].Cor + "," + data[i].Cor2 + ")";
                }
                else {
                    backgroundCor = "url(" + data[i].ImagemCor + "); background-size: 100% 100%;";
                }

                var element = "<span class='btn btn-default btn-sm cor selected'" + " id='" + data[i].ProdutoCorId + "' style='background: " + backgroundCor + "' onclick=Cores(" + data[i].ProdutoCorId + ");></span>";
                if ($("#img-principal") != null) {
                    Cores(data[i].ProdutoCorId);
                }
                $('#div-form-produto.' + data[0].ProdutoId + ' form').find('input#ProdutoCorId').val(data[i].ProdutoCorId);
                $("div#" + data[i].ProdutoId + ".cores").append(element);
                if ($("#img-principal") == null) {
                    var ProdutoId = data[i].ProdutoId;
                    var imagens = {};
                    imagens.url = "/ProdutoVitrine/ObterImagem";
                    imagens.type = "GET";
                    imagens.data = { "corId": data[i].ProdutoCorId };
                    imagens.datatype = "json";
                    imagens.success = function (dataImg) {
                        $('img.' + ProdutoId).attr('src', dataImg.Imagem);
                    }
                    $.ajax(imagens);
                }
            } else {
                var backgroundCor = "";

                if (data[i].ImagemCor == null || data[i].ImagemCor == "") {
                    backgroundCor = "linear-gradient(to left," + data[i].Cor + "," + data[i].Cor2 + ");";
                }
                else {
                    backgroundCor = "url(" + data[i].ImagemCor + "); background-size: 100% 100%;";
                }

                var element = "<span class='btn btn-default btn-sm cor'" + " id='" + data[i].ProdutoCorId + "' style='background: " + backgroundCor + "' onclick=Cores(" + data[i].ProdutoCorId + ");></span>";
                $("div#" + data[i].ProdutoId + ".cores").append(element);
            }
        }
    }
    $.ajax(options);
});

function Cores(id) {
    var options = {};
    options.url = "/ProdutoVitrine/ObterImagem";
    options.type = "GET";
    options.data = { "corId": id };
    options.datatype = "json";
    $('#ProdutoCorId').val(id);
    options.success = function (data) {
        if (data == null || data.length == 0) return;
        var todasAsCores = $("div#" + data[0].ProdutoId + ".cores *");
        for (var i = 0; i < todasAsCores.length; i++) {
            if ($(todasAsCores[i]).attr('class').search('selected') > 1) {
                $(todasAsCores[i]).removeClass('selected');
            }
        }
        for (var i = 0; i < todasAsCores.length; i++) {
            if ($(todasAsCores[i]).attr('id') == id) {
                $(todasAsCores[i]).addClass('selected');
                $('#div-form-produto.' + data[0].ProdutoId + ' form').find('input#ProdutoCorId').val(id);
                $('img.' + data[0].ProdutoId).attr('src', data[0].Imagem);
                $('img.' + data[0].ProdutoId).attr('onClick', "DispararBotao(" + data[0].ProdutoId + ")");
            }
        }

        if ($("#img-principal") != null) {
            $("div#img-secundaria").empty();
            for (var i = 0; i < data.length; i++) {
                if (i == 0) {
                    if (data[i].TipoArquivo == 0) {
                        $("#img-principal").replaceWith("<img id='img-principal' class='img-show' src='" + data[i].Imagem + "' data-zoom-image = '" + data[i].Imagem2 + "' />");
                        $(".img-show").elevateZoom();
                    } else {
                        $("#img-principal").replaceWith("<video id='img-principal' autoplay><source src='" + data[i].Imagem + "' type='video/mp4'/></video>");
                    }
                }

                var elementoImg = "";

                if (data[i].TipoArquivo == 0) {
                    elementoImg = "<img alt='' class='img-responsive" + data[i].ProdutoCorId + "' id='" + data[i].Id + "' src='" + data[i].Imagem + "' src-other = '" + data[i].Imagem2 + "' tipo='Imagem' onmouseover='TrocaImagem(this);' />";
                } else {
                    elementoImg = "<img class='img-responsive" + data[i].ProdutoCorId + "' id='" + data[i].Id + "' tipo = 'Video' url = '" + data[i].Imagem + "' src='/img/no-video-img.png' onmouseover='TrocaImagem(this);'/>";
                }

                $("div#img-secundaria").append(elementoImg);

                $('#div-button-comprar' + data[0].ProdutoId).empty();

                $('#quantidadeEstoque').html(data[0].quantidadeEstoque);

                $('#codReferencia').html(data[0].codReferencia);

                $('#avaliacao').empty();

                $('#avaliacao').append(data[0].porcent);

                $('#AvaliacaoNota').empty();

                $('#AvaliacaoNota').append(data[0].notaEstrelas);

                if ((window.location.pathname.search("Details") > 0) || window.location.pathname.search("Produto") > 0) {

                    var url = "/Produto/" + data[0].ProdutoMontadoId;

                    $("#AvaliacaoCreate").load("/AvaliacaoProdutos/Create?produtoMotadoId=" + data[0].ProdutoMontadoId + "&returnUrl=" + url);

                    $('#AvaliacaoLista').load("/AvaliacaoProdutos/Index?produtoMontadoId=" + data[0].ProdutoMontadoId);

                    window.history.pushState("produto", "Produto", url);
                }

                if (data[0].disponibilidade) {
                    var location = ((window.location.pathname.search("Details") > 0) || window.location.pathname.search("Produto") > 0);
                    if (location) {
                        $('#div-button-comprar' + data[0].ProdutoId).append("<input id='btn-comprar' class='btn-success btn' name='" + data[0].ProdutoId + "' type='submit' value='+ Adicionar ao carrinho' onClick='DisplayWait();' >");
                    } else {
                        $('#div-button-comprar' + data[0].ProdutoId).append("<input id='btn-comprar' class='btn-success btn' name='" + data[0].ProdutoId + "' type='submit' value='Comprar' onClick='DisplayWait();'>");
                    }
                } else {
                    var url = "/ProdutoVitrine/EnviarEmail?ProdutoMontadoId=" + data[0].ProdutoMontadoId;
                    $('#div-button-comprar' + data[0].ProdutoId).append("<a id='btn-comprar' class='btn-warning open-modal btn' href='" + url + "' name='" + data[0].ProdutoId + "'>Produto indisponível Avise-me!</a>");
                }
            }
        }
    }
    $.ajax(options);
    Valor(id);
};

function Valor(id) {
    var options = {};
    options.url = "/ProdutoVitrine/ObterValor";
    options.type = "GET";
    options.data = { "ProdutoCorId": id };
    options.datatype = "json";
    options.success = function (data) {
        if (data == null || data.length == 0) return;
        var elemento = $('#label-valor-produto' + data.ProdutoId);
        var elementoOri = $('#origemlabel-valor-produto' + data.ProdutoId);
        elementoOri.empty();
        if (data.ValueOrigem != null) {
            $(elementoOri).html("De: " + data.ValueOrigem);
            $(elemento).html("Por: " + data.Value);
        }else {
            $(elemento).html(data.Value);
        }
        $('#' + data.ProdutoId).parent().find('.valor-desconto').remove();
        if (data.isPromocao) {
            $('#' + data.ProdutoId).parent().find('.valor-desconto').remove();
            var parentElement = $('#' + data.ProdutoId).parent().find('.panel-body');
            $(parentElement).prepend("<span class= 'valor-desconto text-center' title='Promoção válida apenas para tamanho e cor selecionado!'>" + data.desconto + "% <br/>Off</span>");
            $('#produtoDetails  .img-principal').prepend("<span class= 'valor-desconto text-center' title='Promoção válida apenas para tamanho e cor selecionado!'>" + data.desconto + "% <br/>Off</span>");
        }
    }
    $.ajax(options);
};


function TrocaImagem(obj) {

    var tipo = $(obj).attr("tipo");
    if (tipo == "Imagem") {
        var imgSecund = $(obj).attr("src");
        var imgSecund2 = $(obj).attr("src-other");
        $("#img-principal").replaceWith("<img id='img-principal' class='img-show' src='" + imgSecund + "' data-zoom-image= '" + imgSecund2 + "' />");
    } else {
        var url = $(obj).attr("url");
        $("#img-principal").replaceWith("<video id='img-principal' autoplay><source src='" + url + "' type='video/mp4'></video>");
    }

    $(".img-show").elevateZoom();
};

$(".img-secund").hover(function () {
    var tipo = $(this).attr("tipo");
    if (tipo == "Imagem") {
        var imgSecund = $(this).attr("src");
        var imgSecund2 = $(this).attr("src-other");
        $("#img-principal").replaceWith("<img id='img-principal' class='img-show' src='" + imgSecund + "' data-zoom-image= '" + imgSecund2 + "' />");
    } else {
        var url = $(this).attr("url");
        $("#img-principal").replaceWith("<video id='img-principal' autoplay><source src='" + url + "' type='video/mp4'></video>");
    }
    $(".img-show").elevateZoom();
});

$("#menu-toggle").click(function (e) {
    e.preventDefault();
    $("#menu-toggle").toggle("");
    $("#menu-toggle").toggle("");
    if ($("#menu-toggle").html() == "Fechar") {
        $("#menu-toggle").html("Categorias");
    } else {
        $("#menu-toggle").html("Fechar");
    }
    $("#wrapper").toggleClass("toggled");
});

var typingTimer; //timer identifier
var doneTypingInterval = 1000; //time in ms, 5 second for example

//on keyup, start the countdown
/*
$('#input-search').keyup(function () {
    clearTimeout(typingTimer);
    if ($('#input-search').val) {
        typingTimer = setTimeout(doneTyping, doneTypingInterval);
    }
});
*/

$('#input-search').keyup(function (e) {
    if (e.keyCode == 13) {
        DisplayWait();
        window.location = "/?search=" + $(this).val();
    }
});

function Search_Click() {
    DisplayWait();
    window.location = "/?search=" + $(this).val();
}

//user is "finished typing," do something
function doneTyping() {
    $("#result-search tbody").empty();

    if ($('#input-search').val() === "") {
        $("#result-search").css('display', 'none');
    } else {
        $("#result-search").css('display', 'block');
    }

    var id = $("#input-search").val();
    $.get("/ProdutoVitrine/Search", { "search": id }, function (detailsHtml) {
        $("#result-search tbody").html(detailsHtml);
    });
}

function ReloadGridSearch(pagina) {
    var id = $("#input-search").val();
    $.get("/ProdutoVitrine/Search", { "search": id, "pagina": pagina }, function (detailsHtml) {
        $("#result-search tbody").html(detailsHtml);
    });
}

//$(document).mouseup(function () {
//    $("#result-search").css('display', 'none');
//});

$(document).mouseup(function (e) {
    var $div = $("#result-search"),
        $btn = $("#input-search");

    // se o alvo do clique não é a DIV ou um filho da DIV
    if (!$div.is(e.target) && $div.has(e.target).length === 0) {

        // se o alvo não é o botão que abre/fecha a DIV
        if (!$btn.is(e.target) && $btn.has(e.target).length === 0) {

            // se a DIV está aberta
            if ($div.is(':visible')) {
                $div.slideToggle();
            }
        }
    }
});

jQuery(document).ready(function ($) {
    $(".clickable-row").click(function () {
        window.location = $(this).data("href");
    });
});


function PesquisaCep(elemento) {
    //Nova variável "cep" somente com dígitos.
    var cep = $(elemento).val().replace(/\D/g, '');

    //Verifica se campo cep possui valor informado.
    if (cep != "") {

        //Expressão regular para validar o CEP.
        var validacep = /^[0-9]{8}$/;

        //Valida o formato do CEP.
        if (validacep.test(cep)) {

            //Preenche os campos com "..." enquanto consulta webservice.
            $("input[name = 'Endereco']").val("...");
            $("input[name = 'Rua']").val("...");
            $("input[name = 'Bairro']").val("...");
            $("input[name = 'Cidade']").val("...");
            $("#Uf").val("...");
            $("#Estado").val(0).trigger("chosen:updated");
            $("input[name = 'Ibge']").val("...");

            //Consulta o webservice viacep.com.br/
            $.getJSON("//viacep.com.br/ws/" + cep + "/json/?callback=?", function (dados) {

                if (!("erro" in dados)) {
                    //Atualiza os campos com os valores da consulta.
                    $("input[name = 'Rua']").val(dados.logradouro);
                    $("input[name = 'Endereco']").val(dados.logradouro);
                    $("input[name = 'Bairro']").val(dados.bairro);
                    $("input[name = 'Cidade']").val(dados.localidade);
                    $("#Uf").val(dados.uf);
                    var i = jQuery("#Estado option").filter(function () {
                        return $.trim($(this).text()) == dados.uf
                    }).val();
                    $("#Estado").val(i).trigger("chosen:updated");
                    $("#Ibge").val(dados.ibge);
                } //end if.
                else {
                    //CEP pesquisado não foi encontrado.
                    limpa_formulário_cep();
                    alert("CEP não encontrado.");
                }
            });
        } //end if.
        else {
            //cep é inválido.
            limpa_formulário_cep();
            alert("Formato de CEP inválido.");
        }
    } //end if.
    else {
        //cep sem valor, limpa formulário.
        limpa_formulário_cep();
    }
}

function limpa_formulário_cep() {
    // Limpa valores do formulário de cep.
    $("input[name = 'Endereco']").val("");
    $("input[name = 'Rua']").val("");
    $("input[name = 'Bairro']").val("");
    $("input[name = 'Cidade']").val("");
    $("#Uf").val("");
    $("#Estado").val(0).trigger("chosen:updated");
    $("input[name = 'Ibge']").val("...");
}

$(document).ready(function () {
    //Quando o campo cep perde o foco.
    $("input[name = 'Cep']").blur(function () {
        PesquisaCep(this);
    });
});

$(document).ready(function () {
    try {
        //Quando o campo cep perde o foco.
        $("#Cep").blur(function () {
            Cep(this);
        });
    } catch (e) {

    }
});

$(document).ready(function () {
    $("#btnPesquisaCep").click(function () {
        var $btn = $(this).button('loading')
        var elemento = $("#Cep");
        PesquisaCep(elemento);
        Cep(elemento);
        $btn.button('reset')
    });
});

function Cep(elemento) {
    //Nova variável "cep" somente com dígitos.
    var cep = $(elemento).val().replace(/\D/g, '');

    //Verifica se campo cep possui valor informado.
    if (cep != "") {

        //Expressão regular para validar o CEP.
        var validacep = /^[0-9]{8}$/;

        //Valida o formato do CEP.
        if (validacep.test(cep)) {

            //Preenche os campos com "..." enquanto consulta webservice.
            $("td#CepPrazo").html("consultando ...");
            $("td#CepValor").html("consultando ...");
            $("td#CepValor2").html("consultando ...");
            $("td#CepPrazo2").html("consultando ...");

            //Consulta o webservice viacep.com.br/
            $.getJSON("/Correios/PrazoCep?cepDestino=" + cep, function (dados) {
                //Atualiza os campos com os valores da consulta.
                $("td#CepPrazo").html("Prazo de entrega: " + dados[0].PrazoEntrega + " dia(s)");
                $("td#CepPrazo2").html(dados[0].PrazoEntrega + " dia(s)");
                $("td#CepValor").html("Valor: " + formatReal(dados[0].Valor.replace(",", "")));
                $("td#CepValor2").html("R$: " + formatReal(dados[0].Valor.replace(",", "")));
                $("#ValorFrete").val(parseFloat(dados[0].Valor));

                var totalCarrinho = (parseFloat($("#carrinhoTotal").val()) + parseFloat(dados[0].Valor)).toFixed(2).replace(".", "");

                $("#valorTotal").html("R$ " + formatReal(totalCarrinho.toString()));

            }); //end if.
        }
    }
}

function formatReal(int) {
    var tmp = int + '';
    tmp = tmp.replace(/([0-9]{2})$/g, ",$1");
    if (tmp.length > 6)
        tmp = tmp.replace(/([0-9]{3}),([0-9]{2}$)/g, ".$1,$2");

    return tmp;
}

/*
try {
    var elemento = document.getElementById("Cep");
    Cep(elemento);
    PesquisaCep(elemento);
} catch (e) {

}
*/

function AlterarBoleto() {

    if ($("#spanBoleto").html() == "CLIQUE") {
        $("#spanBoleto").removeClass("label-success");
        $("#spanBoleto").addClass("label-primary");
        $("#spanBoleto").html("Pagamento com boleto bancário");
        $("#panelBoleto").removeClass("invisible");
        $("#panelCartoes").addClass("invisible");
        $("#spanCartaoCredito").html("CLIQUE");
        $("#spanCartaoCredito").addClass("label-success");
        $("#spanCartaoCredito").removeClass("label-primary");
        $("#boleto").prop('checked', true);
        $("#PgtoBoleto").removeClass("invisible");
        $("#DadosCartao").addClass("invisible");
    }
}

function AlterarCartaoCredito() {

    if ($("#spanCartaoCredito").html() == "CLIQUE") {

        $("#boleto").prop('checked', false);
        $("#spanCartaoCredito").removeClass("label-success");
        $("#spanCartaoCredito").addClass("label-primary");
        $("#spanCartaoCredito").html("Selecione o cartão de crédito");
        $("#panelCartoes").removeClass("invisible");
        $("#panelBoleto").addClass("invisible");
        $("#spanBoleto").html("CLIQUE");
        $("#spanBoleto").addClass("label-success");
        $("#spanBoleto").removeClass("label-primary");
        $("#DadosCartao").removeClass("invisible");
        $("#PgtoBoleto").addClass("invisible");
    }
}


$(function () {
    $('body')
        .on('click',
            '.open-modal',
            function (e) {
                $(this).attr('data-target', '#modal-container');
                $(this).attr('data-toggle', 'modal');
                e.preventDefault();
            });

    //clear modal cache, so that new content can be loaded
    $('#modal-container')
        .on('hidden.bs.modal',
            function () {
                $(this).removeData('bs.modal');
            });
});

// Para envio de e-mail de aviso de disponibilidade
function EnviarEmail(url) {

    var parametros =
    {
        Email: $("#Email").val(),
        ProdutoMontadoId: $("#ProdutoMontadoId").val()
    }

    $.post(url, parametros, function (retorno) {
        $("#EnviarEmailDisponivel").empty();
        var msg = "<h3>" + retorno.Mensagem + "</h3>";
        $("#EnviarEmailDisponivel").html(msg);
    });
}

try {
    $(".img-show").elevateZoom();
} catch (e) {

}


$(document).ajaxStart(function () {
    $(document.body).css({ 'cursor': 'wait' });
    $("#wait").css("display", "block");
}).ajaxStop(function () {
    $("#wait").css("display", "none");
    $(document.body).css({ 'cursor': 'default' });
});

(function ($) {
    $(function () {
        $("#Cpf").mask("999.999.999-99");
        $("#DddFixo").mask("99");
        $("#TelefoneFixo").mask("9999-9999");
        $("#DddCelular").mask("99");
        $("#TelefoneCelular").mask("99999-9999");
        $("#Cep").mask("99999-999");
    });
})(jQuery);

//$("#cpfcnpj").unmask();

/*
    Contador para promoção
*/

// Define um intervalo para atualizações das promoções.
//setInterval(updatePromotion, 20000);

var tempo = $("#promotions div.active #sessao").attr('control-value');

$("#promotions").on('slid.bs.carousel', function () {
    tempo = $("#promotions div.active #sessao").attr('control-value');
});

function updatePromotion() {
    $("#slidePromotionHome").load("/Admin/Promotions/SlidePromotion");
    $("#promotions").carousel();
    tempo = $("#promotions div.active #sessao").attr('control-value');
    $("#promotions").on('slid.bs.carousel', function () {
        tempo = $("#promotions div.active #sessao").attr('control-value');
    });
}

function startCountdown() {

    // Se o tempo não for zerado

    if ((tempo - 1) >= 0) {

        // Pega a parte inteira dos minutos

        var min = parseInt(tempo / 60);

        // horas, pega a parte inteira dos minutos
        var hor = parseInt(Math.round(tempo / 60 / 60 % 24));

        //atualiza a variável minutos obtendo o tempo restante dos minutos
        min = min % 60;

        // Calcula os segundos restantes
        var seg = tempo % 60;

        // Formata o número menor que dez, ex: 08, 07, ...

        if (min < 10) {
            min = "0" + min;

            min = min.substr(0, 2);

        }

        seg = parseFloat(seg.toFixed(0));

        if (seg <= 9) {
            seg = "0" + seg;

        }

        if (hor <= 9) {
            hor = "0" + hor;
        }

        // Cria a variável para formatar no estilo hora/cronômetro

        var dias = Math.round(tempo / 60 / 60 / 24);

        var horaImprimivel = "";

        if (dias == 1) {
            horaImprimivel = hor + ' hora ' + min + ' minutos ' + seg + ' segundos ';
        } else {
            horaImprimivel = dias + ' dias ' + hor + ' horas ' + min + ' minutos ' + seg + ' segundos ';
        }

        //JQuery pra setar o valor

        $("#promotions div.active #sessao").html(horaImprimivel);

        // Define que a função será executada novamente em 1000ms = 1 segundo

        setTimeout('startCountdown()', 1000);

        var tempoAtual = $("#promotions div.active #sessao").attr('control-value');
        $("#promotions div.active #sessao").attr('control-value', tempoAtual--);
        // diminui o tempo

        tempo--;

        // Quando o contador chegar a zero faz esta ação
    } else {
        $("#promotions div.active #sessao").html("Promoção encerrada");
    }
}
// Chama a função ao carregar a tela
startCountdown();

/*
$(window).bind('beforeunload', function () {
    $.ajax({
        url: "/Carrinho/VerificarCarrinho"
    });
});
*/

//$(window).unload(function () {
//    $.ajax({
//        url: "/Carrinho/VerificarCarrinho"
//    });
//});


function DispararBotao(botao) {
    $("input[name = '" + botao + "']").trigger('click');
};

$(".dropdown").mouseenter(function () {
    $(this).find(".dropdown-menu").css('display', 'block');
});

$(".dropdown").mouseleave(function () {
    $(this).find(".dropdown-menu").css('display', 'none');
});

$('.panel').mouseenter(function () {
    var footerHeight = $(this).find(".panel-footer").height();
    var oldHeight = this.offsetHeight;
    var oldWidth = $($(this).parent()).width();
    var left = $(this).position().left;
    $(this).find(".panel").attr("style", "height:" + (footerHeight + oldHeight) + "px; width: " + oldWidth + "px;");
    $(this).find(".panel-footer").attr("style", "left:" + (left) + "px;");
});

$('.panel').mouseleave(function () {
    var footerHeight = $(this).find(".panel-footer").height();
    var oldHeight = this.offsetHeight;
    var oldWidth = $($(this).parent()).width();
    var left = $(this).position().left;
    $(this).find(".panel").attr("style", "height:" + (footerHeight + oldHeight) + "px; width: " + oldWidth + "px;");
    $(this).find(".panel-footer").attr("style", "width:" + (oldWidth - 5) + "px; left:" + (left) + "px;");
});

/*
if (window.location.pathname == "/" || window.location.pathname == "/Home/" || window.location.pathname == "/Home/Index" || window.location.pathname == "/Home/Index/") {
    if ($('.modal-pop-up div#slidePromotionHome').html() != "") {
        $('.modal-pop-up').modal('show');
    }
}
*/
//btnAplicaCupom

function DisplayWait() {
    $(document.body).css({ 'cursor': 'wait' });
    $("#wait").css("display", "block");
}

$(".dropdown").on('click', function (data) {
    DisplayWait();
    var href = $(this).find('a').first().attr('href');
    window.location = href;
});