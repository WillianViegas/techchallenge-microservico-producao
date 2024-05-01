﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum EPedidoStatus
    {
        Novo = 0,
        PendentePagamento = 1,
        Recebido = 2,
        EmPreparo = 3,
        Pronto = 4,
        Finalizado = 5
    }
}
