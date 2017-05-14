<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="text" indent="no"  />
  <xsl:strip-space elements="*"/>
  <xsl:preserve-space elements="expression class"/>

  <xsl:param name="cr">
    <xsl:text>&#13;</xsl:text>
  </xsl:param>
  <xsl:param name="nl">
    <xsl:text>&#10;</xsl:text>
  </xsl:param>

  <xsl:template match="regex">
    <xsl:apply-templates select="node()" />
  </xsl:template>

  <xsl:template match="start">
    <xsl:text>^</xsl:text>
  </xsl:template>
  <xsl:template match="end">
    <xsl:text>$</xsl:text>
  </xsl:template>
  <xsl:template match="or">
    <xsl:text>|</xsl:text>
  </xsl:template>
  <!--Adding a carriage return results in another line and adding a new line automatically adds a carriage return in XSLT?!?!-->
  <!--<xsl:template match="cr">
    <xsl:copy-of select="$cr"/>
  </xsl:template>-->
  <xsl:template match="nl">
    <xsl:copy-of select="$nl"/>
  </xsl:template>

  <xsl:template match="expression">
    <xsl:value-of select="."/>
    <xsl:value-of select="@quantifier"/>
  </xsl:template>

  <xsl:template match="class" >
    <xsl:text>[</xsl:text>
    <xsl:value-of select="."/>
    <xsl:text>]</xsl:text>
    <xsl:value-of select="@quantifier"/>
  </xsl:template>

  <xsl:template match="set" >
    <xsl:text>(?</xsl:text>
    <xsl:value-of select="@original"/>
    <xsl:text>)</xsl:text>
  </xsl:template>

  <xsl:template match="comment">
    <xsl:if test="not(@xmode = 'true')">
      <xsl:text>(?#</xsl:text>
      <xsl:value-of select="."/>
      <xsl:text>)</xsl:text>
    </xsl:if>
    <xsl:if test="@xmode = 'true'">
      <xsl:text>#</xsl:text>
      <xsl:value-of select="."/>
    </xsl:if>
  </xsl:template>

  <xsl:template match="namedBackref" >
    <xsl:if test="not(@negated = 'true')">
      <xsl:text>\k&lt;</xsl:text>
      <xsl:value-of select="."/>
      <xsl:text>&gt;</xsl:text>
    </xsl:if>
    <xsl:if test="@negated = 'true'">
      <xsl:text>\K&lt;</xsl:text>
      <xsl:value-of select="."/>
      <xsl:text>&gt;</xsl:text>
    </xsl:if>
    <xsl:value-of select="@quantifier"/>
  </xsl:template>

  <xsl:template match="namedClass" >
    <xsl:if test="not(@negated = 'true')">
      <xsl:text>\p{</xsl:text>
      <xsl:value-of select="."/>
      <xsl:text>}</xsl:text>
    </xsl:if>
    <xsl:if test="@negated = 'true'">
      <xsl:text>\P{</xsl:text>
      <xsl:value-of select="."/>
      <xsl:text>}</xsl:text>
    </xsl:if>
    <xsl:value-of select="@quantifier"/>
  </xsl:template>

  <xsl:template match="group" >
    <xsl:text>(</xsl:text>

    <xsl:if test="@options">
      <xsl:text>?</xsl:text>
      <xsl:value-of select="@options"/>:
    </xsl:if>
    <xsl:if test="@nonGrouping">
      <xsl:text>?:</xsl:text>
    </xsl:if>
    <xsl:if test="@lookAround='positive,forward'">
      <xsl:text>?=</xsl:text>
    </xsl:if>
    <xsl:if test="@lookAround='positive,backward'">
      <xsl:text>?&lt;=</xsl:text>
    </xsl:if>
    <xsl:if test="@lookAround='negative,forward'">
      <xsl:text>?!</xsl:text>
    </xsl:if>
    <xsl:if test="@lookAround='negative,backward'">
      <xsl:text>?&lt;!</xsl:text>
    </xsl:if>
    <xsl:if test="@atomic='true'">
      <xsl:text>?&gt;</xsl:text>
    </xsl:if>
    <xsl:if test="@conditional">
      <xsl:text>?</xsl:text>
    </xsl:if>
    <xsl:if test="@name">
      <xsl:if test="not(@nameChar=&quot;'&quot;)">
        <xsl:text>?&lt;</xsl:text>
        <xsl:value-of select="@name"/>
        <xsl:text>&gt;</xsl:text>
      </xsl:if>
      <xsl:if test="@nameChar=&quot;'&quot;">
        <xsl:text>?'</xsl:text>
        <xsl:value-of select="@name"/>
        <xsl:text>'</xsl:text>
      </xsl:if>
    </xsl:if>

    <xsl:if test="not(.='')">
      <xsl:apply-templates select="node()"/>
    </xsl:if>

    <xsl:text>)</xsl:text>
    <xsl:value-of select="@quantifier"/>
  </xsl:template>
</xsl:stylesheet>
