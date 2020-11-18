namespace Softplan.Justica.GerenciadorProcessos.Domain.Constants
{
    public class ErrorMessages
    {
        public const string ProcessoJaExiste = "Número do processo {0} em uso.";
        public const string ProcessoNaoEcontrado = "Processo de número {0} não encontrado.";
        public const string ProcessoQuantidadeHierarquiaExcedido = "Número de processos na hierarquia excedido. Máximo 4.";
        public const string ProcessoJaConstaNaHierarquia = "Processo já existente na hierarquia.";
        public const string ProcessoFinalizado = "Processo {0} finalizado.";
        public const string ProcessoPaiNaoPodeSerRemovido = "Processo pai não pode ser removido.";

        public const string SituacaoNaoEncontrada = "Situação Processo {0} não encontrada.";

        public const string ResponsavelNaoEncontrado = "Um ou mais responsaveis não encontrados.";
        public const string ResponsavelVinculadoProcesso = "Usuário vinculado à um processo.";

        public const string CpfEmUso = "Cpf {0} em uso.";
        public const string CpfInvalido = "Cpf {0} inválido.";
        public const string EmailEmUso = "Email {0} em uso.";
        public const string EmailInvalido = "Email {0} inválido.";

        public const string ErroInesperado = "Erro inesperado.";
        public const string ErroVazio = "Valor não pode ser vazio.";
        public const string ErroTamanhoMaximo = "Tamanho máximo ({0}).";
        public const string ErroTamanhoMinimo = "Tamanho mínimo ({0}).";
        public const string ErroDataDeveSerAnterior = "Data deve ser anterior à {0}.";
        public const string ErroQuantidadeResponsavel = "Valor deve estar entre {0} e {1}.";
        public const string ResponsavelRepetido = "Responsáveis repetidos não são permitidos";
    }
}