// Adaptador de jQuery Validation para el atributo [FechaPosteriorA] (Models/FechaPosteriorAAttribute.cs).
// Lee los data-val-fechaposteriora-* que genera el servidor y valida en el navegador sin ir al servidor.
(function ($) {
    $.validator.addMethod("fechaposteriora", function (value, element, params) {
        var fechaInicial = $(params.form).find("[name='" + params.propiedad + "']").val();
        if (!value || !fechaInicial) {
            return true;
        }
        var dias = (new Date(value) - new Date(fechaInicial)) / 86400000;
        return dias >= params.dias;
    });

    $.validator.unobtrusive.adapters.add("fechaposteriora", ["propiedad", "dias"], function (options) {
        options.rules["fechaposteriora"] = {
            propiedad: options.params.propiedad,
            dias: parseInt(options.params.dias, 10),
            form: options.form
        };
        options.messages["fechaposteriora"] = options.message;

        // Si cambia la fecha inicial, se vuelve a validar la final.
        $(options.form).find("[name='" + options.params.propiedad + "']").on("change", function () {
            $(options.element).valid();
        });
    });
})(jQuery);
