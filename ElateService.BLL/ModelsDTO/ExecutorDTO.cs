﻿using ElateService.Common;
using System.Collections.Generic;

namespace ElateService.BLL.ModelsDTO
{
    public class ExecutorDTO
    {
        public int ExecutorId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string Information { get; set; }
        public string ImgSrc { get; set; }
        public int? Mark { get; set; }

        public ISet<Category> Categories { get; set; }
        public ISet<IndentDTO> Indents { get; set; }
        public ISet<ResponceDTO> Responces { get; set; }
    }
}
