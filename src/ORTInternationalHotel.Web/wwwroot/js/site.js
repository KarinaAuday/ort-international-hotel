// Buscador en vivo para las tablas del ABM.
// Cualquier <input data-table-search="idDeLaTabla"> filtra las filas de esa tabla
// por coincidencia de texto (sin distinguir mayusculas/minusculas ni acentos).
(function () {
    function normalizar(texto) {
        return texto
            .toLowerCase()
            .normalize("NFD")
            .replace(/[̀-ͯ]/g, "");
    }

    function filtrarTabla(input) {
        var tabla = document.getElementById(input.getAttribute("data-table-search"));
        if (!tabla) {
            return;
        }

        var filas = tabla.querySelectorAll("tbody tr[data-row]");
        var termino = normalizar(input.value.trim());
        var visibles = 0;

        filas.forEach(function (fila) {
            var texto = normalizar(fila.textContent || "");
            var coincide = termino === "" || texto.indexOf(termino) !== -1;
            fila.classList.toggle("is-hidden", !coincide);
            if (coincide) {
                visibles++;
            }
        });

        var vacio = tabla.querySelector(".empty-row");
        if (vacio) {
            vacio.classList.toggle("is-hidden", visibles !== 0);
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll("input[data-table-search]").forEach(function (input) {
            input.addEventListener("input", function () {
                filtrarTabla(input);
            });
        });
    });
})();
