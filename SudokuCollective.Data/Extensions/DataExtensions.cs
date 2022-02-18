using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using SudokuCollective.Core.Enums;
using SudokuCollective.Core.Interfaces.Models.DomainObjects.Payloads;
using SudokuCollective.Core.Models;
using SudokuCollective.Data.Models.Payloads;

namespace SudokuCollective.Data.Extensions
{
    public static class DataExtensions
    {
        public static bool ConvertToPayloadSuccessful(this JsonElement element, Type type, out IPayload result)
        {
            result = null;

            if (type == typeof(AppPayload))
            {
                try
                {
                    result = new AppPayload()
                    {
                        Name = element.GetProperty("name").ToString(),
                        LocalUrl = element.GetProperty("localUrl").ToString(),
                        DevUrl = element.GetProperty("devUrl").ToString(),
                        QaUrl = element.GetProperty("qaUrl").ToString(),
                        ProdUrl = element.GetProperty("prodUrl").ToString(),
                        IsActive = Convert.ToBoolean(element.GetProperty("isActive").ToString()),
                        Environment = (ReleaseEnvironment)Convert.ToInt32(element.GetProperty("environment").ToString()),
                        PermitSuperUserAccess = Convert.ToBoolean(element.GetProperty("permitSuperUserAccess").ToString()),
                        PermitCollectiveLogins = Convert.ToBoolean(element.GetProperty("permitCollectiveLogins").ToString()),
                        DisableCustomUrls = Convert.ToBoolean(element.GetProperty("disableCustomUrls").ToString()),
                        CustomEmailConfirmationAction = element.GetProperty("customEmailConfirmationAction").ToString(),
                        CustomPasswordResetAction = element.GetProperty("customPasswordResetAction").ToString(),
                        TimeFrame = (TimeFrame)Convert.ToInt32(element.GetProperty("timeFrame").ToString()),
                        AccessDuration = Convert.ToInt32(element.GetProperty("accessDuration").ToString())
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(CreateDifficultyPayload))
            {
                try
                {
                    result = new CreateDifficultyPayload()
                    {
                        Name = element.GetProperty("name").ToString(),
                        DisplayName = element.GetProperty("displayName").ToString(),
                        DifficultyLevel = (DifficultyLevel)Convert.ToInt32(element.GetProperty("difficultyLevel").ToString())
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(UpdateDifficultyPayload))
            {
                try
                {
                    result = new UpdateDifficultyPayload()
                    {
                        Id = Convert.ToInt32(element.GetProperty("id").ToString()),
                        Name = element.GetProperty("name").ToString(),
                        DisplayName = element.GetProperty("displayName").ToString()
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(AnnonymousCheckPayload))
            {
                try
                {
                    result = new AnnonymousCheckPayload()
                    {
                        FirstRow = element.GetProperty("firstRow").ToString().ToCharArray().ToList().ConvertAll(c => Convert.ToInt32(c)),
                        SecondRow = element.GetProperty("secondRow").ToString().ToCharArray().ToList().ConvertAll(c => Convert.ToInt32(c)),
                        ThirdRow = element.GetProperty("thirdRow").ToString().ToCharArray().ToList().ConvertAll(c => Convert.ToInt32(c)),
                        FourthRow = element.GetProperty("fourthRow").ToString().ToCharArray().ToList().ConvertAll(c => Convert.ToInt32(c)),
                        FifthRow = element.GetProperty("fifthRow").ToString().ToCharArray().ToList().ConvertAll(c => Convert.ToInt32(c)),
                        SixthRow = element.GetProperty("sixthRow").ToString().ToCharArray().ToList().ConvertAll(c => Convert.ToInt32(c)),
                        SeventhRow = element.GetProperty("seventhRow").ToString().ToCharArray().ToList().ConvertAll(c => Convert.ToInt32(c)),
                        EighthRow = element.GetProperty("eighthRow").ToString().ToCharArray().ToList().ConvertAll(c => Convert.ToInt32(c)),
                        NinthRow = element.GetProperty("ninthRow").ToString().ToCharArray().ToList().ConvertAll(c => Convert.ToInt32(c))
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(AnnonymousGamePayload))
            {
                try
                {
                    result = new AnnonymousGamePayload()
                    {
                        DifficultyLevel = (DifficultyLevel)Convert.ToInt32(element.GetProperty("difficultyLevel").ToString())
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(CreateGamePayload))
            {
                try
                {
                    result = new CreateGamePayload()
                    {
                        UserId = Convert.ToInt32(element.GetProperty("userId").ToString()),
                        DifficultyId = Convert.ToInt32(element.GetProperty("difficultyId").ToString())
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(GamesPayload))
            {
                try
                {
                    result = new GamesPayload()
                    {
                        UserId = Convert.ToInt32(element.GetProperty("userId").ToString())
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(UpdateGamePayload))
            {
                try
                {
                    var cells = new List<SudokuCell>();

                    var cellsArray = element.GetProperty("sudokuCells");

                    foreach (JsonElement number in cellsArray.EnumerateArray())
                    {
                        var cell = new SudokuCell(
                            Convert.ToInt32(number.GetProperty("id").ToString()),
                            Convert.ToInt32(number.GetProperty("index").ToString()),
                            Convert.ToInt32(number.GetProperty("column").ToString()),
                            Convert.ToInt32(number.GetProperty("region").ToString()),
                            Convert.ToInt32(number.GetProperty("row").ToString()),
                            Convert.ToInt32(number.GetProperty("value").ToString()),
                            Convert.ToInt32(number.GetProperty("displayedValue").ToString()),
                            Convert.ToBoolean(number.GetProperty("hidden").ToString()),
                            Convert.ToInt32(number.GetProperty("sudokuMatrixId").ToString())
                            );

                        cells.Add(cell);
                    }
                    // TO DO: add logic to convert sudoku cells
                    result = new UpdateGamePayload()
                    {
                        GameId = Convert.ToInt32(element.GetProperty("gameId").ToString()),
                        SudokuCells = cells
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(CreateRolePayload))
            {
                try
                {
                    result = new CreateRolePayload()
                    {
                        Name = element.GetProperty("name").ToString(),
                        RoleLevel = (RoleLevel)Convert.ToInt32(element.GetProperty("roleLevel").ToString())
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(UpdateRolePayload))
            {
                try
                {
                    result = new UpdateRolePayload()
                    {
                        Id = Convert.ToInt32(element.GetProperty("id").ToString()),
                        Name = element.GetProperty("name").ToString(),
                        RoleLevel = (RoleLevel)Convert.ToInt32(element.GetProperty("roleLevel").ToString())
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(AddSolutionPayload))
            {
                try
                {
                    result = new AddSolutionPayload()
                    {
                        Limit = Convert.ToInt32(element.GetProperty("limit").ToString())
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(SolutionPayload))
            {
                try
                {
                    var firstRow = new List<int>();
                    var secondRow = new List<int>();
                    var thirdRow = new List<int>();
                    var fourthRow = new List<int>();
                    var fifthRow = new List<int>();
                    var sixthRow = new List<int>();
                    var seventhRow = new List<int>();
                    var eighthRow = new List<int>();
                    var ninthRow = new List<int>();

                    var firstArray = element.GetProperty("firstRow");

                    foreach (JsonElement number in firstArray.EnumerateArray())
                    {
                        firstRow.Add(number.GetInt32());
                    }

                    var secondArray = element.GetProperty("secondRow");

                    foreach (JsonElement number in secondArray.EnumerateArray())
                    {
                        secondRow.Add(number.GetInt32());
                    }

                    var thirdArray = element.GetProperty("thirdRow");

                    foreach (JsonElement number in thirdArray.EnumerateArray())
                    {
                        thirdRow.Add(number.GetInt32());
                    }

                    var fourthArray = element.GetProperty("fourthRow");

                    foreach (JsonElement number in fourthArray.EnumerateArray())
                    {
                        fourthRow.Add(number.GetInt32());
                    }

                    var fifthArray = element.GetProperty("fifthRow");

                    foreach (JsonElement number in fifthArray.EnumerateArray())
                    {
                        fifthRow.Add(number.GetInt32());
                    }

                    var sixthArray = element.GetProperty("sixthRow");

                    foreach (JsonElement number in sixthArray.EnumerateArray())
                    {
                        sixthRow.Add(number.GetInt32());
                    }

                    var seventhArray = element.GetProperty("seventhRow");

                    foreach (JsonElement number in seventhArray.EnumerateArray())
                    {
                        seventhRow.Add(number.GetInt32());
                    }

                    var eighthArray = element.GetProperty("eighthRow");

                    foreach (JsonElement number in eighthArray.EnumerateArray())
                    {
                        eighthRow.Add(number.GetInt32());
                    }

                    var ninthArray = element.GetProperty("ninthRow");

                    foreach (JsonElement number in ninthArray.EnumerateArray())
                    {
                        ninthRow.Add(number.GetInt32());
                    }

                    result = new SolutionPayload()
                    {
                        FirstRow = firstRow,
                        SecondRow = secondRow,
                        ThirdRow = thirdRow,
                        FourthRow = fourthRow,
                        FifthRow = fifthRow,
                        SixthRow = sixthRow,
                        SeventhRow = seventhRow,
                        EighthRow = eighthRow,
                        NinthRow = ninthRow
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(PasswordResetPayload))
            {
                try
                {
                    result = new PasswordResetPayload()
                    {
                        UserId = Convert.ToInt32(element.GetProperty("userId").ToString()),
                        NewPassword = element.GetProperty("newPassword").ToString()
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(RequestPasswordResetPayload))
            {
                try
                {
                    result = new RequestPasswordResetPayload()
                    {
                        License = element.GetProperty("license").ToString(),
                        Email = element.GetProperty("email").ToString()
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(ResetPasswordPayload))
            {
                try
                {
                    result = new ResetPasswordPayload()
                    {
                        Token = element.GetProperty("token").ToString(),
                        NewPassword = element.GetProperty("newPassword").ToString()
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else if (type == typeof(UpdateUserPayload))
            {
                try
                {
                    result = new UpdateUserPayload()
                    {
                        UserName = element.GetProperty("userName").ToString(),
                        FirstName = element.GetProperty("firstName").ToString(),
                        LastName = element.GetProperty("lastName").ToString(),
                        NickName = element.GetProperty("nickName").ToString(),
                        Email = element.GetProperty("email").ToString()
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                try
                {
                    result = new UpdateUserRolePayload()
                    {
                        RoleIds = element.GetProperty("roleIds").ToString().ToCharArray().ToList().ConvertAll(c => Convert.ToInt32(c))
                    };

                    return true;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
