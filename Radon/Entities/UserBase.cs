using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Radon.Models;
using Radon.Security;

namespace Radon;

/// <summary>
/// The user base is the base class for every user model. It contains the track history for <see cref="RefreshToken"/>s.
/// The real design of the user model can be defined the developer using Radon.
/// </summary>
[ModelBinder(BinderType = typeof(UserModelBinder))]
public abstract class UserBase
{
    /// <summary>
    /// The id of the user identifying it in the database.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    /// <summary>
    /// The track history for <see cref="RefreshToken"/>s.
    /// </summary>
    [JsonIgnore] 
    public List<RefreshToken> RefreshTokens = new();

    /// <summary>
    /// Starts the voting process for this user with the given subject. If a <see cref="Voter{TU,TS}"/> denies
    /// the authorization, an unauthorized response will be issued.
    /// </summary>
    /// <param name="subject">The subject for the voter.</param>
    public void VoteFor(object subject)
    {
        VoterCollection.Vote(this, subject);
    }
}