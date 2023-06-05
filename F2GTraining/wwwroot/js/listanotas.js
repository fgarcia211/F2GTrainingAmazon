function muestraOpciones(idjugador) {
    console.log(idjugador)
    var divisor = $("#sessionopt-" + idjugador).slideToggle(250);
}

function vuelveInicio() {
    window.location.href = "/equipos/MenuEquipo";
}

function listaNotas() {
    document.getElementsByClassName("view-sessions-zone")[0].style.display = "none";
    Swal.fire({
        title: "Nombre de la Nota",
        html:
            '<input type="text"/><br/>' +
            '<input type="text"/>',
        background: '#111111',
        color: "#CFC0FF",
        showCancelButton: true,
        confirmButtonText: "Guardar Notas",
        cancelButtonText: "Cancelar",
        allowOutsideClick: () => {
            return false
        }
    }).then(resultado => {
        console.log(resultado)
        if (resultado.isConfirmed) {
            $('#titulo').val(resultado.value);
            $('#cuerpo').val(resultado.value);
            $('#form-new-nota').submit();
        }
        else {
            document.getElementsByClassName("view-sessions-zone")[0].style.display = "block"
        }
    });
}