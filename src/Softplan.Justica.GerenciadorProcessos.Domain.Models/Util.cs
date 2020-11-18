using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Softplan.Justica.GerenciadorProcessos.Domain.Models
{
    // https://www.eximiaco.tech/pt/2019/06/11/validacao-de-cpf/
    public static class Util
    {
        public struct NumeroProcesso
        {
            private readonly string value;

            private NumeroProcesso(string numeroProcesso)
            {
                this.value = new string(numeroProcesso.Where(Char.IsDigit).ToArray());
            }

            public static implicit operator NumeroProcesso(string value) => new NumeroProcesso(value);

            public override string ToString() => this.value;
        }

        public struct Email
        {
            private readonly string email;
            public readonly bool Valido;

            private Email(string email)
            {
                this.email = email;

                var regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.CultureInvariant | RegexOptions.Singleline);

                this.Valido = regex.IsMatch(this.email);
            }

            public static implicit operator Email(string value) => new Email(value);

            public override string ToString() => this.email;
        }

        public struct Cpf
        {
            private readonly string value;
            public readonly bool Valido;

            private Cpf(string value)
            {
                if (value == null)
                {
                    this.Valido = false;
                    this.value = string.Empty;
                    return;
                }

                this.value = new string(value.Where(Char.IsDigit).ToArray());

                var posicao = 0;
                var totalDigito1 = 0;
                var totalDigito2 = 0;
                var dv1 = 0;
                var dv2 = 0;

                bool digitosIdenticos = true;
                var ultimoDigito = -1;

                foreach (var c in value)
                {
                    if (char.IsDigit(c))
                    {
                        var digito = c - '0';
                        if (posicao != 0 && ultimoDigito != digito)
                        {
                            digitosIdenticos = false;
                        }

                        ultimoDigito = digito;
                        if (posicao < 9)
                        {
                            totalDigito1 += digito * (10 - posicao);
                            totalDigito2 += digito * (11 - posicao);
                        }
                        else if (posicao == 9)
                        {
                            dv1 = digito;
                        }
                        else if (posicao == 10)
                        {
                            dv2 = digito;
                        }

                        posicao++;
                    }
                }

                if (posicao > 11)
                {
                    Valido = false;
                    return;
                }

                if (digitosIdenticos)
                {
                    Valido = false;
                    return;
                }

                var digito1 = totalDigito1 % 11;
                digito1 = digito1 < 2 ? 0 : 11 - digito1;

                if (dv1 != digito1)
                {
                    Valido = false;
                    return;
                }

                totalDigito2 += digito1 * 2;
                var digito2 = totalDigito2 % 11;
                digito2 = digito2 < 2 ? 0 : 11 - digito2;

                Valido = dv2 == digito2;
            }

            public static implicit operator Cpf(string value) => new Cpf(value);

            public string Formatado => Convert.ToUInt64(this.ToString()).ToString(@"000\.000\.000\-00");

            public override string ToString() => value;
        }

        public static bool ValidarCpf(Cpf sourceCpf) => sourceCpf.Valido;

        public static bool ValidarEmail(Email sourceEmail) => sourceEmail.Valido;
    }
}