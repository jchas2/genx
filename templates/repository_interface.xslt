<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:md="http://genx.com/metadata" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="text" encoding="UTF-8" indent="yes" />
<xsl:preserve-space elements="*"/>

<xsl:param name="EntityName" />
<xsl:param name="OrganisationName" />
<xsl:param name="ApplicationName" />

<xsl:template match="/">
   <xsl:apply-templates select="//md:entities/md:entity[@name=$EntityName]/md:entitycolumns" />
</xsl:template>

<xsl:template match="md:entitycolumns">
<xsl:variable name="EntityName" select="ancestor::md:entity/@name"/>
using <xsl:value-of select="$ApplicationName"/>.Core.Entities;
using <xsl:value-of select="$OrganisationName"/>.Core.Repository;

namespace Comtrac.Core.Repository
{
    public interface I<xsl:value-of select="$EntityName"/>Repository : IRepository&lt;<xsl:value-of select="$EntityName" />&gt;
    {
    }
}
</xsl:template>
</xsl:stylesheet> 
