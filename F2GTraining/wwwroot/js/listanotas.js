function muestraOpciones(idjugador) {
    console.log(idjugador)
    var divisor = $("#sessionopt-" + idjugador).slideToggle(250);
}

function vuelveInicio() {
    window.location.href = "/equipos/MenuEquipo";
}

function listaNotas() {
    document.getElementsByClassName("view-notes-zone")[0].style.display = "none";
    Swal.fire({
        title: "Nombre de la Nota",
        html:
            '<label>Titulo de nota: </label><br/><input style="margin-top: 20px; margin-bottom:20px" type="text" id="swaltitulo"/><br/>' +
            '<label>Contenido: </label></br><input style="margin-top: 20px; margin-bottom:20px type="text" id="swalcuerpo"/>',
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
            $('#titulo').val($("#swaltitulo").val());
            $('#cuerpo').val($("#swalcuerpo").val());
            $('#form-new-nota').submit();
        }
        else {
            document.getElementsByClassName("view-notes-zone")[0].style.display = "block"
        }
    });
}