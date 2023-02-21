﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Models.Enum;

namespace Techgen.Domain.Entities.Identity
{
    public class VerificationToken : IEntity
    {
        #region Properties

        public int Id { get; set; }

        public int? UserId { get; set; }

        [DefaultValue("")]
        public string PhoneNumber { get; set; }

        [MaxLength(200)]
        [DefaultValue("")]
        public string TokenHash { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsUsed { get; set; }

        public VerificationCodeType Type { get; set; }

        /// <summary>
        /// Additional data in Json Format
        /// </summary>
        public string Data { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("UserId")]
        [InverseProperty("VerificationTokens")]
        public virtual ApplicationUser User { get; set; }

        #endregion

        #region Additional Properties

        [NotMapped]
        public bool IsValid
        {
            get
            {
                return (DateTime.UtcNow - CreateDate).TotalMinutes < 5;
            }
        }

        #endregion
    }
}
